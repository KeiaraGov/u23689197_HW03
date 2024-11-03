using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u23689197_HW03.Models;
using PagedList;
using PagedList.Mvc;

namespace u23689197_HW03.Controllers
{
    public class HomeController : Controller
    {
        LibraryDBContext db = new LibraryDBContext();
        public async Task<ActionResult> Index(int? pageStudents, int? pageBooks)
        {
            var model = new CombinedViewModel
            {
                students = (await db.students.ToListAsync()).ToPagedList(pageStudents ?? 1, 10),
                books = (await db.books
                .Include(b => b.type)
                .Include(b => b.author)
                .ToListAsync()).ToPagedList(pageBooks ?? 1, 10),
                borrows = await db.borrows.ToListAsync()
            };
            return View(model);
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

        public async Task<ActionResult> Maintain(int? pageTypes, int? pageAuthors, int? pageBorrows)
        {
            int pageSize = 10; // Set page size as desired

            var model = new MaintainViewModel
            {
                Types = (await db.types.ToListAsync()).ToPagedList(pageTypes ?? 1, pageSize),
                Authors = (await db.authors.ToListAsync()).ToPagedList(pageAuthors ?? 1, pageSize),
                Borrows = (await db.borrows
                    .Include(b => b.book)
                    .Include(b => b.student)
                    .ToListAsync()).ToPagedList(pageBorrows ?? 1, pageSize)
            };

            return View(model);
        }

        public ActionResult Report()
        {
            ViewBag.Message = "Your report page.";

            return View();
        }
    }
}