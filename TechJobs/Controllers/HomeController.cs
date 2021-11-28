using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobs.Models;
using TechJobs.ViewModels;
using Microsoft.AspNetCore.Identity;
using TechJobs.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using TechJobs.Services;




namespace TechJobs.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext context;
        

        public HomeController(ApplicationDbContext dbContext)
        {
            context = dbContext;
            
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        [Authorize(Roles = "manager")]
        [HttpGet("/Add")]
        public IActionResult AddJob(AddJobViewModel addJobViewModel)
        {
            
            addJobViewModel.SelectListItem = context.Employers.ToList();
            addJobViewModel.PossibleSkills = context.Skills.ToList();

            return View(addJobViewModel);
        }

        [Authorize(Roles = "manager")]
        public IActionResult ProcessAddJobForm(AddJobViewModel addJobViewModel, string[] selectedSkills)
        {
            if (ModelState.IsValid)
            {
                Job job = new Job(addJobViewModel.Name, addJobViewModel.EmployerId);
                context.Jobs.Add(job);

               
                context.SaveChanges();
                int i = 0;
                List<Job> list = context.Jobs.Include(j=>j.Employer).ToList();

                

                foreach (Job j in list)
                {
                    if (j == job)
                    {
                        i = j.Id;
                        break;
                    }
                }

                foreach (string s in selectedSkills)
                {
                   
                    JobSkill newSkill = new JobSkill(job.Id, int.Parse(s));
                   
                    context.JobSkills.Add(newSkill);

                    
                }
               
                context.SaveChanges();

                

                List<ApplicationUser> users = context.Users.ToList();
                Job newestJob = context.Jobs.Where(j => j.Name == job.Name).Where(j => j.EmployerId == job.EmployerId).Include(j => j.Employer).ToList()[0];


                Emailer.Notify(users, newestJob);
             



                return Redirect("/Home/");
            }

            return View("AddJob", addJobViewModel);


          
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }


        public IActionResult JobsByMyLocation()
        {
            List<Job> jobs = context.Jobs.ToList();
            List<Employer> employers = context.Employers.ToList();
            List<Job> userJobs = new List<Job>();
            ApplicationUser user = context.Users.Where(u => u.UserName == User.Identity.Name).ToList()[0];

            foreach (Employer e in employers)
            {
                if (e.Location.ToLower() == user.Location.ToLower())
                {
                    foreach (Job job in jobs)
                    {
                        if (e.Id == job.EmployerId)
                        {
                            userJobs.Add(job);
                        }
                    }
                }
            }

            string bodyHTML = "<img src='https://i.imgur.com/SJg5nzm.png' style='width: 170px; height: auto; border-radius: 15px;'></br><h2>Here are the best job opportunities available to you!</h1></br><ul>";

            for (int i = 0; i < userJobs.Count; i++)
            {
                bodyHTML += "<li>" + userJobs[i].Name + ", " + userJobs[i].Employer.Name + ", " + userJobs[i].Employer.Location + "</li></br>";
            }

            bodyHTML += "</ul>";

            Emailer.LocationEmail(bodyHTML, user);

            List<Job> jobs1 = context.Jobs.Include(j => j.Employer).ToList();



            return View("Index", jobs1);
        }
    }
}
