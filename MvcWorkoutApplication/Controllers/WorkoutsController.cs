using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWorkoutApplication.Controllers
{
    public class WorkoutsController : Controller
    {
        private db_appEntities db = new db_appEntities();

        //
        // GET: /Workouts/

        public ActionResult Index()
        {
            return View(db.workouts.ToList());
        }

        //
        // GET: /Workouts/Details/5

        public ActionResult Details(string id = null)
        {
            workout workout = db.workouts.Find(id);
            if (workout == null)
            {
                return HttpNotFound();
            }
            return View(workout);
        }

        //
        // GET: /Workouts/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Workouts/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(workout workout)
        {
            if (ModelState.IsValid)
            {
                db.workouts.Add(workout);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(workout);
        }

        //
        // GET: /Workouts/Edit/5

        public ActionResult Edit(string id = null)
        {
            workout workout = db.workouts.Find(id);
            if (workout == null)
            {
                return HttpNotFound();
            }
            return View(workout);
        }

        //
        // POST: /Workouts/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(workout workout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workout);
        }

        //
        // GET: /Workouts/Delete/5

        public ActionResult Delete(string id = null)
        {
            workout workout = db.workouts.Find(id);
            if (workout == null)
            {
                return HttpNotFound();
            }
            return View(workout);
        }

        //
        // POST: /Workouts/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            workout workout = db.workouts.Find(id);
            db.workouts.Remove(workout);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}