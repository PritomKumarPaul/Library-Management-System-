using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        LMSEntities4 db = new LMSEntities4();

        //GET: Home

        public ActionResult Index()
        {
            if (Session["AdminName"] == null && Session["Name"] == null)
            {
                ViewBag.Notification = "Login first!";
                return View();
            }
            //return View(db.Books.ToList());
            //return RedirectToAction("Index", "Books");
            return View();
        }

        public ActionResult Search(string search)
        {
            return View(db.Books.Where(x => x.Title.Contains(search) || search == null).ToList());
        }

        /*public ActionResult Exit(int? id)
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
            if (Session["Id"] != null)
            {
                DateTime now = DateTime.Now;
                monitor.Exit_Time = DateTime.Now;
                db.Entry(monitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Search");
            }
            return View(monitor);
        }*/

        public ActionResult IssueCreate(string Title)
        {
            if (Title.Equals(null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Where(x => x.Title.Equals(Title)).FirstOrDefault();
            Issue issue = new Issue();
            if (Session["Id"] != null)
            {
                int mid = Convert.ToInt32(Session["Id"]);
                int count = db.Issues.Where(x => x.Id == mid && x.Return_Status.Equals("false")).Count();
                if (count >= 3)
                {
                    return RedirectToAction("Create", "Issues");
                }
                DateTime thisDay = DateTime.Today;
                issue.Title = Title;
                issue.Issue_Date = thisDay;
                issue.Return_Status = "false";
                issue.Taken = "false";
                issue.DeadLine = thisDay.AddDays(7);
                issue.Id = Convert.ToInt32(Session["Id"]);
                db.Issues.Add(issue);
                db.SaveChanges();
                return RedirectToAction("Search");
            }
            if (book == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Search");
        }

        public ActionResult Give(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            Approve approve = new Approve();
            if (Session["AdminName"] != null)
            {
                DateTime thisDay = DateTime.Today;
                approve.Issue_Id = id;
                approve.Admin_Id = Convert.ToInt32(Session["AdminId"]);
                approve.Approve_Time = thisDay;
                //issue.Return_Status = "false";
                db.Approves.Add(approve);
                db.SaveChanges();
                issue.Taken = "true";
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexGive", "Issues");
            }
            if (issue == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", "Issues");
        }

        public ActionResult Return(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            Reutrn ret = new Reutrn();
            if (Session["AdminName"] != null)
            {
                DateTime thisDay = DateTime.Today;
                ret.Issue_Id = id;
                ret.Admin_Id = Convert.ToInt32(Session["AdminId"]);
                ret.Approve_Time = thisDay;
                //issue.Return_Status = "false";
                db.Reutrns.Add(ret);
                db.SaveChanges();
                issue.Return_Status = "true";
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexReturn", "Issues");
            }
            if (issue == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", "Issues");
        }

        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUp(Member member)
        {
            int value;
            if (db.Members.Any(x=>x.AustId == member.AustId))
            {
                ViewBag.Notification = "This Id is already in use.";
                return View();
            }
            else if (member.Name==null || !Regex.Match(member.Name, @"^[\p{L} \.\-]+$").Success)
            {
                ViewBag.Notification = "Enter name correctly.";
                return View();
            }
            else if (member.Email == null || !Regex.Match(member.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                ViewBag.Notification = "Enter e-mail correctly.";
                return View();
            }
            else if (member.Phone_Number == null || !Regex.Match(member.Phone_Number, @"^(?:\+88|88)?(01[3-9]\d{8})$").Success)
            {
                ViewBag.Notification = "Enter phone number correctly.";
                return View();
            }
            else if(member.AustId.Length!=9)
            {
                ViewBag.Notification = "Id should be 9 digits.";
                return View();
            }
            else if (!int.TryParse(member.AustId, out value))
            {
                ViewBag.Notification = "Id should contain digits only.";
                return View();
            }
            else if ((value / 100000) % 100 < 1 || (value / 100000) % 100 > 2 ||
                    (value / 1000) % 100 == 0 || (value / 1000) % 100 > 8 ||
                    value % 1000 == 0)
            {
                
                
                    ViewBag.Notification = "This Id is invalid.";
                    return View();
                //Abc100@#

            }
            else if (member.Password == null || !Regex.Match(member.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$").Success)
            {
                ViewBag.Notification = "Password should include minimum eight characters, at least one letter, one number and one special character";
                return View();
            }
            else if(!member.Password.Equals(member.RePassword))
            {
                ViewBag.Notification = "Re-entered password doesn't match!";
                return View();
            }
            else
            {
                //member.AdminId = 1;
                //member.Status = "no";
                DateTime thisDay = DateTime.Today;
                member.JoiningDate = thisDay;
                db.Members.Add(member);
                db.SaveChanges();
                Session["Id"] = member.Id.ToString();
                Session["Name"] = member.Name.ToString();
                Session["AustId"] = member.AustId.ToString();
                ViewBag.Notification = "Welcome member!";
                return RedirectToAction("Index", "Home");
            }
            return View();     
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin admin)
        {
            var checklogin = db.Admins.Where(x => x.AdminName.Equals(admin.AdminName) && x.Password.Equals(admin.Password)).FirstOrDefault();
            if(checklogin!=null)
            {
                /*var adminlist = db.Admins.ToList();
                var idlist = new SelectList(db.Admins, "Id");
                //var idlist = (from AdminId in db.Admins select AdminId).ToList();
                var namelist = new SelectList(db.Admins, "AdminName");
                //var namelist = (from AdminName in db.Admins select AdminName).ToList();
                for (int i=0;i<adminlist.Count;i++)
                {
                    if(namelist.ElementAt(i).Equals(admin.AdminName))
                    {
                        Session["AdminId"] = idlist.ElementAt(i);
                    }
                }*/
                Admin admin1 = db.Admins.Where(x => x.AdminName.Equals(admin.AdminName)).FirstOrDefault();
                Session["AdminId"] = admin1.AdminId.ToString();
                Session["AdminName"] = admin.AdminName.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Wrong username or password!";
            }
            return View();
        }


        [HttpGet]
        public ActionResult LoginMember()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginMember(Member member)
        {
            var chechlogin = db.Members.Where(x => x.AustId.Equals(member.AustId) && x.Password.Equals(member.Password)).FirstOrDefault();
            if (chechlogin != null)
            {
                Member m1 = db.Members.Where(x => x.AustId.Equals(member.AustId)).FirstOrDefault();
                Session["Id"] = m1.Id.ToString();
                Session["Name"] = m1.Name.ToString();
                Session["AustId"] = m1.AustId.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Wrong username or password!";
            }
            return View();
        }
    }
}