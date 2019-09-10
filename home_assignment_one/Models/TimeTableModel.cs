using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace home_assignment_one.Models
{
    public class TimeTableModel
    {
        public int employee_id { get; set; }
        public int billable_rate { get; set; }
        public string project_name { get; set; }
        public string date_created { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public int time_difference
        {
            get
            {
                return Convert.ToInt32((TimeSpan.Parse(end_time) - (TimeSpan.Parse(start_time))).TotalHours);
            }

        }
        public int cost
        {
            get
            {
                return time_difference * billable_rate;
            }

        }
    }
    public static class ViewExtensions
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

            var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }
    }
}