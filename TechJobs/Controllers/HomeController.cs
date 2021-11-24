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

namespace TechJobs.Controllers
{
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

                List<Job> job1 = context.Jobs.Where(j => j.Name == job.Name).Where(j => j.EmployerId == job.EmployerId).Include(j => j.Employer).ToList();

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

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("techjobspersistent@gmail.com", "LaunchCode75?"),
                    EnableSsl = true,
                };


                List<IdentityUser> users = context.Users.ToList();



                foreach (IdentityUser u in users)
                {
                    string bodyHTML = "<h3>A new job for you was just posted!</h3>";
                    bodyHTML += "<p>" + job1[0].Name + ", " + job1[0].Employer.Name + ", " + job1[0].Employer.Location + "</p>";
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("techjobspersistent@gmail.com"),
                        Subject = "New job posting!",
                        Body = bodyHTML,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(u.Email);

                    smtpClient.Send(mailMessage);
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
    }
}
