using ClosedXML.Excel;
using LAB3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Syncfusion.XlsIO;
using System.IO;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace LAB4.Controllers
{
    public class HandlerController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IHostingEnvironment _environment;
        private readonly IConverter _converter;

        public HandlerController(AppDbContext dbContext, IHostingEnvironment environment, IConverter converter)
        {
            _dbContext = dbContext;
            _environment = environment;
            _converter = converter;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32 != null)
                return View();
            else
                RedirectToAction("Login", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult ProcessingPdf(string url)
        {
            var webRootPath = _environment.WebRootPath;

			var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                    //Out = Path.Combine(webRootPath, "/pdf/", "/test.pdf/"),
                },
                Objects = {
                    new ObjectSettings()
                    {
                        Page = url,
                    },
                }
            };

            var t = _converter.Convert(doc);
			var time = DateTime.Now;
			if (t != null)
            {            
                using (var stream = new FileStream(Path.Combine(webRootPath, @"pdf/", time.ToString("dd_mm_yyyy_hh_mm_ss") + ".pdf"), FileMode.Create, FileAccess.Write,FileShare.None))              //Lỗi dòng này anh ơi
                {
					stream.Write(t, 0, t.Length);

					var historyNew = new HistoryItem(url, time, "Success");

                    historyNew.userRefId = HttpContext.Session.GetInt32("id");
					_dbContext.HistoryItem.Add(historyNew);
                    _dbContext.SaveChanges();

					ViewBag.Status = "Success";
                    ViewBag.FileName = time.ToString("dd_mm_yyyy_hh_mm_ss") + ".pdf";
                }    
			}
            else
            {
				var historyNew = new HistoryItem(url, time, "Error");

				historyNew.userRefId = HttpContext.Session.GetInt32("id");
				_dbContext.HistoryItem.Add(historyNew);
                _dbContext.SaveChanges();

				ViewBag.Status = "Error";
			} 
                

			return View("Index");
        }

        [HttpPost]
        public FileResult DownloadFile(string filename)
        {
			var webRootPath = _environment.WebRootPath;

			string path = Path.Combine(webRootPath, @"pdf/", filename);
            byte[] file = System.IO.File.ReadAllBytes(path);

			return File(file, "application/octet-stream", filename);
        }

        public IActionResult History()
        {
            List<HistoryItem> items = _dbContext.HistoryItem.Where(p => p.userRefId == HttpContext.Session.GetInt32("id")).ToList();

            return View("History", items);
        }

        public FileResult ExportXlsx()
        {
			var webRootPath = _environment.WebRootPath;
            var time = DateTime.Now.ToString("dd_mm_yyyy_hh_mm_ss") + ".xlsx";
            byte[] file;

			using (var workbook = new XLWorkbook())
			{
				var items = _dbContext.HistoryItem.Where(p => p.userRefId == HttpContext.Session.GetInt32("id")).ToList();
				int rownum = 2;

				var worksheet = workbook.Worksheets.Add("History Sheet");

				foreach (var item in items)
				{
                    worksheet.Cell("A" + rownum).Value = rownum-1;
					worksheet.Cell("B" + rownum).Value = item.Url;
					worksheet.Cell("C" + rownum).Value = item.TimeAct.ToString("dd-MM-yyyy hh:mm:ss");
					worksheet.Cell("D" + rownum).Value = item.Status;
					rownum++;
				}

				var workbookBytes = new byte[0];
				using (var ms = new MemoryStream())
				{
					workbook.SaveAs(ms);
					workbookBytes = ms.ToArray();
				}

				file = workbookBytes;
			}

			

			//using (ExcelEngine excelEngine = new ExcelEngine())
   //         {
			//	IApplication application = excelEngine.Excel;
			//	application.DefaultVersion = ExcelVersion.Xlsx;

			//	IWorkbook workbook = application.Workbooks.Create(1);
			//	IWorksheet worksheet = workbook.Worksheets[0];

                

			//	worksheet.Range["A" + rownum].Text = "STT";
			//	worksheet.Range["B" + rownum].Text = "Url được chuyển đổi";
			//	worksheet.Range["C" + rownum].Text = "Thời điểm chuyển đổi";
			//	worksheet.Range["D" + rownum].Text = "Trạng thái";

				

   //             workbook.SaveAs(new FileStream(Path.Combine(webRootPath, @"/xlsx/", time), FileMode.Create, FileAccess.Write,FileShare.None));
			//}

            
            return File(file, "application/octet-stream", "History.xlsx");
		}
    }
}
