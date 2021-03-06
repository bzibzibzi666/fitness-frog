﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };
            SetUpActivitiesViewbag();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            if(ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                TempData["Message"] = "Successfully added";
                return RedirectToAction("Index");
            }
            SetUpActivitiesViewbag();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entry = _entriesRepository.GetEntry((int)id);
            if(entry==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            SetUpActivitiesViewbag();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                TempData["Message"] = "Successfully edited";
                return RedirectToAction("Index");
            }
            SetUpActivitiesViewbag();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entry = _entriesRepository.GetEntry((int)id);
            if (entry == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(entry);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            _entriesRepository.DeleteEntry((int)id);
            TempData["Message"] = "Successfully deleted";
            return RedirectToAction("Index");
        }

        private void SetUpActivitiesViewbag()
        {
            ViewBag.ActivitySelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }

    }
}