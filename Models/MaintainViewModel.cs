using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;

namespace u23689197_HW03.Models
{
    public class MaintainViewModel
    {
        public IPagedList<type> Types { get; set; }
        public IPagedList<author> Authors { get; set; }
        public IPagedList<borrow> Borrows { get; set; }
    }
}