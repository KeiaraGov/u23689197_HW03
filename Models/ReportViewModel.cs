using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u23689197_HW03.Models
{
    public class ReportViewModel
    {
        public List<BookBorrowedInfo> BookBorrows { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }  // Add this property

        public class BookBorrowedInfo
        {
            public string BookTitle { get; set; }
            public int BorrowCount { get; set; }
        }
    }

    public class Report
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }  // Add this property
    }


}