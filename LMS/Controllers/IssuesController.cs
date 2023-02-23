using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;

namespace LMS.Controllers
{
    public class IssuesController : Controller
    {
        private LMSEntities4 db = new LMSEntities4();

        // GET: Issues
        public ActionResult Index()
        {
            var issues = db.Issues.Include(i => i.Member);
            return View(issues.ToList());
        }
        public ActionResult IndexGive()
        {
            var issues = db.Issues.Include(i => i.Member);
            return View(issues.ToList());
        }
        public ActionResult IndexReturn()
        {
            var issues = db.Issues.Include(i => i.Member);
            return View(issues.ToList());
        }

        // GET: Issues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        // GET: Issues/Create
        public ActionResult Create()
        {
            Book book = new Book();
            var booklist = db.Books.ToList();
            ViewBag.booklist = new SelectList(booklist, "Title", "Title");
            ViewBag.Id = new SelectList(db.Members, "Id", "Id");
            //ViewBag.Title = new SelectList(db.Members, "Id", "Title");
            return View();
        }

        // POST: Issues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Issue_Id,Id,Title,Issue_Date,Return_Status")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                //int count = db.Database.ExecuteSqlCommand("select * from Issues where Id="+ Session["Id"] + " and [Return Status]='false'");
                //var is1 = db.Issues.SqlQuery("select * from Issues where Id=1 and [Return Status]='false'").ToList();
                //var count = is1.Count();
                int mid = Convert.ToInt32(Session["Id"]);
                int count = db.Issues.Where(x => x.Id== mid  && x.Return_Status.Equals("false")).Count();
                //int count = db.Database.ExecuteSqlCommand("select * from Issues where Id= 1 and [Return Status]='false'");
                //var count = db.Database.SqlQuery<String>("select count(*) from Issues where Id=" + Session["Id"] + " and [Return Status]='false' as number").ToList();
                //var query=db.Database.SqlQuery<String> ("select * from Issues where Id=" + Session["Id"] + " and [Return Status]='false'").ToList();
                //int count = query.Count();
                /*var issuelist = db.Issues.ToList();
                 *
                var idlist = new SelectList(issuelist, "Id");
                var status = new SelectList(issuelist, "Return_Status");
                int n = issuelist.Count,counter=0;
                for(int i=0;i<n;i++)
                {
                    if (idlist.ElementAt(i) == Session["Id"] && status.ElementAt(i).Equals("false"))
                    {
                        counter++;
                    }
                }*/
                if (count>=3)
                {
                    ViewBag.Notification = "Max number of books issued!";
                    //return RedirectToAction("Index", "Home");
                    return View();
                }
                Book book = db.Books.Where(x => x.Title == issue.Title).FirstOrDefault() ;
                //book.Title = issue.Title;
                var copies=book.Number_of_Copies;
                int usedcopies = db.Issues.Where(x => x.Title == issue.Title && x.Return_Status.Equals("false")).Count();
                if (copies-usedcopies <= 0)
                {
                    ViewBag.Notification = "Sorry! No more copies left for the book.Try again later.";
                    //return RedirectToAction("Index", "Home");
                    return View();
                }
                if (Session["Id"]!=null)
                {
                    DateTime thisDay = DateTime.Today;
                    issue.Issue_Date = thisDay;
                    issue.Return_Status = "false";
                    issue.Taken = "false";
                    issue.DeadLine = thisDay.AddDays(7);
                    issue.Id = Convert.ToInt32(Session["Id"]);
                } else if (Session["AdminName"] != null)
                {
                    DateTime thisDay = DateTime.Today;
                    issue.Issue_Date = thisDay;
                    issue.Return_Status = "false";
                }
                db.Issues.Add(issue);
                db.SaveChanges();
                ViewBag.Notification = "Created successfully!";
            }

            ViewBag.Id = new SelectList(db.Members, "Id", "Id", issue.Id);
            var booklist = db.Books.ToList();
            ViewBag.booklist = new SelectList(booklist, "Title", "Title");
            return View(issue);
        }

        // GET: Issues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "Id", issue.Id);
            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Issue_Id,Id,Title,Issue_Date,Return_Status")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Members, "Id", "Id", issue.Id);
            return View(issue);
        }

        // GET: Issues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Issue issue = db.Issues.Find(id);
            db.Issues.Remove(issue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
