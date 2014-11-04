﻿using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using Logic.DAL;
using Logic.Helpers;
using Logic.Models;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;

namespace Logic.Controllers
{
    public class WorkShopApplicationController : SurfaceController
    {
        #region Actions

        [HttpPost]
        public ActionResult Send(WorkShopApplication model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return CurrentUmbracoPage();
                }
                using (var context = new DataContext())
                {
                    context.WorkShopApplications.Add(model);
                    context.SaveChanges();
                }
                TempData.Add("Application received", true);
            }
            catch (Exception e)
            {
                LogHelper.Error(GetType(), e.ToString(), e);
            }
            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult GetApplications()
        {
            byte[] result;
            using (var context = new DataContext())
            {
                var applications = context.WorkShopApplications.ToList();
                result = CsvConverter<WorkShopApplication>.Convert(applications);
            }

            Response.Clear();
            var contentDisposition = new ContentDisposition
            {
                Inline = false,
                FileName = "Applications.csv",
            };
            Response.AddHeader("Content-Disposition", contentDisposition.ToString());
            var preamble = Encoding.UTF8.GetPreamble();
            return File(preamble.Concat(result).ToArray(), "text/csv");
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            return PartialView("WorkShopApplication", new WorkShopApplication());
        }

        #endregion
    }
}
