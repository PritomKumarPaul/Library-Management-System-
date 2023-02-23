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
    public class monitorsController : Controller
    {
        private LMSEntities4 db = new LMSEntities4();


        public ActionResult Select()
        {
            return View();
        }

        public ActionResult SearchEnter(string search)
        {
            return View(db.Members.Where(x => x.Id.ToString().Contains(search) || search == null).ToList());
        }
        public ActionResult Index(string search)
        {
            return View(db.monitors.Where(x => x.Member_Id.ToString().Contains(search) || search == null).ToList());
        }
        // GET: monitors
        /*public ActionResult Index()
        {
            var monitors = db.monitors.Include(m => m.Admin).Include(m => m.Member);
            return View(monitors.ToList());
        }*/

        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            monitor monitor = new monitor();
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            int count = db.monitors.Where(x => x.Member_Id == monitor.Member_Id && x.Exit_Time.Equals(null)).Count();
            if (count > 0)
            {
                ViewBag.Notification = "Member has already entered!";
                return RedirectToAction("SearchEnter");
            }
            else
            {
                monitor.Admin_Id = Convert.ToInt32(Session["AdminId"]);
                DateTime now = DateTime.Now;
                monitor.Arrival_time = DateTime.Now;
                monitor.Member_Id = id;
                db.monitors.Add(monitor);
                db.SaveChanges();
                ViewBag.Notification = "Entry Successful!";
                return RedirectToAction("SearchEnter");
            }
        }

        //Exit
        public ActionResult Exit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            monitor monitor = db.monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            if (Session["AdminId"] != null)
            {
                DateTime now = DateTime.Now;
                monitor.Exit_Time = DateTime.Now;
                db.Entry(monitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: monitors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            monitor monitor = db.monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // GET: monitors/Create
        public ActionResult Create()
        {
            ViewBag.Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId");
            ViewBag.Member_Id = new SelectList(db.Members, "Id", "Id");
            return View();
        }

        // POST: monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Arrival_time,Exit_Time,Admin_Id,Member_Id")] monitor monitor)
        {
            if (ModelState.IsValid)
            {
                int count = db.monitors.Where(x => x.Member_Id == monitor.Member_Id && x.Exit_Time.Equals(null)).Count();
                if (count > 0)
                {
                    ViewBag.Notification = "Member has already entered!";
                }
                else
                {
                    monitor.Admin_Id = Convert.ToInt32(Session["AdminId"]);
                    DateTime now = DateTime.Now;
                    monitor.Arrival_time = DateTime.Now;
                    db.monitors.Add(monitor);
                    db.SaveChanges();
                    ViewBag.Notification = "Entry Successful!";
                    ViewBag.Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId", monitor.Admin_Id);
                    ViewBag.Member_Id = new SelectList(db.Members, "Id", "Id", monitor.Member_Id);
                    return View(monitor);
                }
            }

            ViewBag.Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId", monitor.Admin_Id);
            ViewBag.Member_Id = new SelectList(db.Members, "Id", "Id", monitor.Member_Id);
            return View(monitor);
        }

        


        // GET: monitors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            monitor monitor = db.monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            ViewBag.Admin_Id = new SelectList(db.Admins, "AdminId", "Password", monitor.Admin_Id);
            ViewBag.Member_Id = new SelectList(db.Members, "Id", "AustId", monitor.Member_Id);
            return View(monitor);
        }

        // POST: monitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Arrival_time,Exit_Time,Admin_Id,Member_Id")] monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Admin_Id = new SelectList(db.Admins, "AdminId", "Password", monitor.Admin_Id);
            ViewBag.Member_Id = new SelectList(db.Members, "Id", "AustId", monitor.Member_Id);
            return View(monitor);
        }

        // GET: monitors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            monitor monitor = db.monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // POST: monitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            monitor monitor = db.monitors.Find(id);
            db.monitors.Remove(monitor);
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
