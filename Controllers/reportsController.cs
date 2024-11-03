using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using u23689197_HW03.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace u23689197_HW03.Controllers
{
    public class ReportsController : Controller
    {
        private readonly LibraryDBContext db = new LibraryDBContext();
        private readonly string _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedReports");

        public ReportsController()
        {
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        // GET: Reports/PopularReport
        public ActionResult PopularReport()
        {
            var report = new ReportViewModel
            {
                BookBorrows = db.borrows
                    .GroupBy(b => b.book.name)
                    .Select(g => new ReportViewModel.BookBorrowedInfo
                    {
                        BookTitle = g.Key,
                        BorrowCount = g.Count()
                    })
                    .OrderBy(b => b.BorrowCount) // Sort by BorrowCount in ascending order
                    .ToList()
            };

            var files = Directory.GetFiles(_saveDirectory)
                                 .Select(f => Path.GetFileName(f))
                                 .ToList();

            var descriptions = files
                .Where(f => f.EndsWith(".txt"))
                .ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => System.IO.File.ReadAllText(Path.Combine(_saveDirectory, f)));

            ViewBag.ArchivedFiles = files.Where(f => !f.EndsWith(".txt")).ToList();
            ViewBag.Descriptions = descriptions;

            return View(report);
        }

        // POST: Reports/SaveReport
        [HttpPost]
        public ActionResult SaveReport(ReportViewModel model, string ChartImage)
        {
            string filePath = Path.Combine(_saveDirectory, $"{model.FileName}.{model.FileType.ToLower()}");

            model.BookBorrows = db.borrows
                .GroupBy(b => b.book.name)
                .Select(g => new ReportViewModel.BookBorrowedInfo
                {
                    BookTitle = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderBy(b => b.BorrowCount) // Sort by BorrowCount in ascending order
                .ToList();

            try
            {
                if (model.FileType == "PDF")
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        if (!string.IsNullOrEmpty(ChartImage))
                        {
                            byte[] imageBytes = Convert.FromBase64String(ChartImage.Replace("data:image/png;base64,", ""));
                            var chartImage = iTextSharp.text.Image.GetInstance(imageBytes);
                            chartImage.ScaleToFit(500f, 300f);
                            pdfDoc.Add(chartImage);
                        }

                        if (!string.IsNullOrEmpty(model.Description))
                        {
                            pdfDoc.Add(new Paragraph("Description:"));
                            pdfDoc.Add(new Paragraph(model.Description));
                        }

                        pdfDoc.Close();
                    }
                }
                else if (model.FileType == "CSV")
                {
                    var csvData = "Book Title,Borrow Count\n" +
                                  string.Join("\n", model.BookBorrows.Select(b => $"{b.BookTitle},{b.BorrowCount}"));
                    System.IO.File.WriteAllText(filePath, csvData);
                    System.IO.File.WriteAllText(Path.Combine(_saveDirectory, $"{model.FileName}.txt"), model.Description);
                }
                else if (model.FileType == "PNG")
                {
                    if (!string.IsNullOrEmpty(ChartImage))
                    {
                        byte[] imageBytes = Convert.FromBase64String(ChartImage.Replace("data:image/png;base64,", ""));
                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                    }
                    System.IO.File.WriteAllText(Path.Combine(_saveDirectory, $"{model.FileName}.txt"), model.Description);
                }

                TempData["Message"] = "Report saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error saving report: {ex.Message}";
            }

            return RedirectToAction("PopularReport");
        }

        // GET: Reports/DownloadReport
        public ActionResult DownloadReport(string fileName)
        {
            string filePath = Path.Combine(_saveDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
                return HttpNotFound();

            string mimeType = fileName.EndsWith(".pdf") ? "application/pdf" : fileName.EndsWith(".csv") ? "text/csv" : "image/png";
            return File(filePath, mimeType, fileName);
        }

        // POST: Reports/DeleteReport
        [HttpPost]
        public ActionResult DeleteReport(string fileName)
        {
            string filePath = Path.Combine(_saveDirectory, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                TempData["Message"] = $"{fileName} deleted successfully.";
            }
            else
            {
                TempData["Error"] = $"{fileName} not found.";
            }

            string descriptionPath = Path.Combine(_saveDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}.txt");
            if (System.IO.File.Exists(descriptionPath))
            {
                System.IO.File.Delete(descriptionPath);
            }

            return RedirectToAction("PopularReport");
        }
    }
}
