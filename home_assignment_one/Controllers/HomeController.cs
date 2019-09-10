using CsvHelper;
using home_assignment_one.Models;
using OfficeOpenXml;
using Pechkin;
using Pechkin.Synchronized;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace home_assignment_one.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadTimeTable()
        {
            List<TimeTableModel> saveuplList = new List<TimeTableModel>();
            try
            {
                var rc = Request.Files;
                if (rc != null)
                {
                    var file = rc[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var stream = file.InputStream;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        if (ext != ".csv")
                        {
                            return Json(new { RespCode = 1, RespMessage = "Please Upload Using .csv file" });
                        }

                        
                        using (var reader = new StreamReader(file.InputStream))
                        using (var csv = new CsvReader(reader))
                        {
                            var records = csv.GetRecords<TimeTableModel>();
                            saveuplList = records.ToList();
                        }
                       
                        
                        Session["Salary"] = saveuplList;
                        var html = PartialView("_timeTableUpld", saveuplList).RenderToString();
                        return Json(new { data_html = html, RespCode = 0, RespMessage = "Please Upload Using .csv file" });
                    }
                }
            }
            catch (SqlException ex)
            {
                return Json(new { data = saveuplList, RespCode = 1, RespMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { data = saveuplList, RespCode = 1, RespMessage = ex.Message });
            }
            return Json(new { data = saveuplList, RespCode = 0, RespMessage = "File Uploaded Successfully" });
        }
        [HttpPost]
        public JsonResult GetSummaryFilesPdf(string companyName) //new
        {
            return Json(new { companyName = companyName });
        }
        public ActionResult DownloadInvoice(string company_name)
        {
            try
            {
                if (Session["Salary"] == null)
                {
                    return Json(new { RespCode = 1, RespMessage = "Kindly upload the file again" });
                }
                else
                {
                    List<TimeTableModel> TimeTableModel = Session["Salary"] as List<TimeTableModel>;
                    StreamReader reader = new StreamReader(Server.MapPath("~/template/test_template.html"));
                    string data = reader.ReadToEnd();
                    string table_row = string.Empty;
                    decimal total_price = 0;
                    foreach(var item in TimeTableModel)
                    {
                        total_price += item.cost;
                        table_row += "<tr style = 'text-align:right;'> " +
                                    " <td>"+ item.employee_id +"</td> " +
                                    " <td>" + item.time_difference + "</td> " +
                                    " <td>" + item.billable_rate + "</td> " +
                                    " <td>" + item.cost + "</td> " +
                                    " </tr> ";

                    }
                    table_row += "<tr> "+
                                 " <td colspan = '3' style = 'text-align:right;font-weight:bold;' > Total </td> " +
                                 "  <td style = 'text-align:right;font-weight:bold;'>" + total_price+"</td> "+
                                 " </tr> ";

                    data = data.Replace("{tr_data}", table_row);
                    data = data.Replace("{company_name}", company_name);

                    GlobalConfig gc = new GlobalConfig();

                    gc.SetMargins(new Margins(100, 100, 100, 100))
                        .SetDocumentTitle(company_name + "_"+string.Format("{0:yyyyMMdd}", DateTime.Now))
                        .SetPaperSize(PaperKind.Letter);
                    
                    IPechkin pechkin = new SynchronizedPechkin(gc);
                    ObjectConfig configuration = new ObjectConfig();



                    configuration
                    .SetAllowLocalContent(true)
                    .SetFallbackEncoding(Encoding.ASCII)
                    .SetLoadImages(true)
                    .SetPrintBackground(true)
                    .SetScreenMediaType(true)
                    .SetCreateExternalLinks(true);

                    byte[] pdfContent = pechkin.Convert(configuration, data);

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + company_name + "_" + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".pdf; size={0}", pdfContent.Length));
                    Response.BinaryWrite(pdfContent);
                    return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}