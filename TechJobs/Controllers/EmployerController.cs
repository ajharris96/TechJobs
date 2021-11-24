using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechJobs.Data;
using TechJobs.Models;
using TechJobs.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechJobs.Controllers
{
    public class EmployerController : Controller
    {
        private ApplicationDbContext context;

        public EmployerController(ApplicationDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Employer> employers = context.Employers.ToList();

            return View(employers);
            
        }

        [Authorize(Roles = "manager")]
        public IActionResult Add()
        {
            AddEmployerViewModel viewModel = new AddEmployerViewModel();
            return View(viewModel);
        }


        [Authorize(Roles = "manager")]
        public IActionResult ProcessAddEmployerForm(AddEmployerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Employer employer = new Employer(viewModel.Name, viewModel.Location, viewModel.Url);
                context.Employers.Add(employer);
                context.SaveChanges();
                return Redirect("/Employer/");
            }

            return View("Add", viewModel);


        }

        public IActionResult About(int id)
        {
            

            List<Employer>employers=context.Employers
                .Where(e => e.Id==id)
                .ToList();

            Employer employer = employers[0];

         

            return View(employer);
        }
    }
}
