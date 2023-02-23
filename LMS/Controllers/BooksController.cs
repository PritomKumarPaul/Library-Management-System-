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
    public class BooksController : Controller
    {
        private LMSEntities4 db = new LMSEntities4();

        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Admin);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            /*if(Session["AdminId"].Equals(null))
            {
                return RedirectToAction("Index", "Home");
            }*/
            ViewBag.Issue_Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Book_Id,Author,Price,Title,Number_of_Copies,Category,Arrival_Date")] Book book)
        {
            if (ModelState.IsValid)
            {
                //int value;
                if (db.Books.Any(x => x.Title == book.Title))
                {
                    ViewBag.Notification = "This book is already recorded.";
                    return View();
                }
                book.Issue_Admin_Id = Convert.ToInt16(Session["AdminId"]);
                Console.WriteLine(book.Issue_Admin_Id);
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Issue_Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId", book.Issue_Admin_Id);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Issue_Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId", book.Issue_Admin_Id);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_Id,Author,Price,Title,Number_of_Copies,Category,Issue_Admin_Id,Arrival_Date")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Issue_Admin_Id = new SelectList(db.Admins, "AdminId", "AdminId", book.Issue_Admin_Id);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
