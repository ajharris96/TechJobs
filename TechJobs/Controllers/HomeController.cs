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
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TechJobs.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext context;
        private readonly EmailSender _emailSender;

        public HomeController(ApplicationDbContext dbContext, EmailSender emailSender)
        {
            context = dbContext;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            ApplicationUser user = context.Users.Where(u => u.UserName == User.Identity.Name).ToList()[0];

            List<UserSkill> userSkills = context.UserSkills.Where(u => u.UserId == user.Id).Include(u => u.Skill).ToList();

            List<JobSkill> jobSkills = new List<JobSkill>();

            List<Job> jobs = new List<Job>();

            foreach (var s in userSkills)
            {
                jobSkills.AddRange(context.JobSkills
                        .Where(j => j.Skill.Name == s.Skill.Name)
                        .Include(j => j.Job)
                        .ToList());
            }

            foreach (var job in jobSkills)
            {
                Job foundJob = context.Jobs
                    .Include(j => j.Employer)
                    .Single(j => j.Id == job.JobId);

                jobs.Add(foundJob);
            }




            var rnd = new Random();
            List<Job> jobs1 = jobs.OrderBy(item => rnd.Next()).ToList();


            return View(jobs1);
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
        public async Task<IActionResult> ProcessAddJobFormAsync(AddJobViewModel addJobViewModel, string[] selectedSkills)
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

                foreach (var u in users)
                {
                    if (u.Notify)
                    {
                        await _emailSender.NotifyAsync(u, newestJob);
                    }
                }
                
             



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


        public async Task<IActionResult> JobsByMyLocationAsync()
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

            await _emailSender.LocationEmailAsync(userJobs, user.Email);


            return RedirectToAction("Index");
        }
    }
}
