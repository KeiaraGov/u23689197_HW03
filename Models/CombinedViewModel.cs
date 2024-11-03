using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;

namespace u23689197_HW03.Models
{
    public class CombinedViewModel
    {
        public IPagedList<student> students {  get; set; }
        public IPagedList<book> books { get; set; }
        public IEnumerable<borrow> borrows { get; set; }
    }
} 