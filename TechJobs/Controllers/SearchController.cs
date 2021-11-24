using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechJobs.Models;
using TechJobs.Data;
using Microsoft.EntityFrameworkCore;
using TechJobs.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechJobs.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationDbContext context;

        public SearchController(ApplicationDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            if (ViewBag.check == null)
            {
                ViewBag.check = "all";
            }
            ViewBag.columns = ListController.ColumnChoices;
            return View();
        }

        public IActionResult Results(string searchType, string searchTerm)
        {
            List<Job> jobs;
            List<JobDetailViewModel> displayJobs = new List<JobDetailViewModel>();

            if (string.IsNullOrEmpty(searchTerm))
            {
                jobs = context.Jobs
                   .Include(j => j.Employer)
                   .ToList();

                foreach (var job in jobs)
                {
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(js => js.JobId == job.Id)
                        .Include(js => js.Skill)
                        .ToList();

                    JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills);
                    displayJobs.Add(newDisplayJob);
                }
            }
            else
            {
                if (searchType == "employer")
                {
                    jobs = context.Jobs
                        .Include(j => j.Employer)
                        .Where(j => j.Employer.Name.ToLower().Contains(searchTerm.ToLower()))
                        .ToList();

                    foreach (Job job in jobs)
                    {
                        List<JobSkill> jobSkills = context.JobSkills
                        .Where(js => js.JobId == job.Id)
                        .Include(js => js.Skill)
                        .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills);
                        displayJobs.Add(newDisplayJob);
                    }

                }
                else if (searchType == "skill")
                {
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(j => j.Skill.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Include(j => j.Job)
                        .ToList();

                    foreach (var job in jobSkills)
                    {
                        Job foundJob = context.Jobs
                            .Include(j => j.Employer)
                            .Single(j => j.Id == job.JobId);

                        List<JobSkill> displaySkills = context.JobSkills
                            .Where(js => js.JobId == foundJob.Id)
                            .Include(js => js.Skill)
                            .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(foundJob, displaySkills);
                        displayJobs.Add(newDisplayJob);
                    }
                }
                else if (searchType == "all")
                {
                    jobs = context.Jobs.Include(j => j.Employer).Where(j => j.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                    jobs.AddRange(context.Jobs.Include(j => j.Employer).Where(j => j.Employer.Name.ToLower().Contains(searchTerm.ToLower())).ToList());
                    jobs.AddRange(context.Jobs.Include(j => j.Employer).Where(j => j.Employer.Location.ToLower().Contains(searchTerm.ToLower())).ToList());
                    List<JobSkill> jobSkills = context.JobSkills
                        .Where(j => j.Skill.Name.ToLower().Contains(searchTerm.ToLower()))
                        .Include(j => j.Job)
                        .ToList();
                    foreach (var job in jobSkills)
                    {
                        Job foundJob = context.Jobs
                            .Include(j => j.Employer)
                            .Single(j => j.Id == job.JobId);

                        List<JobSkill> displaySkills = context.JobSkills
                            .Where(js => js.JobId == foundJob.Id)
                            .Include(js => js.Skill)
                            .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(foundJob, displaySkills);
                        displayJobs.Add(newDisplayJob);
                    }

                    foreach (var job in jobs)
                    {
                        List<JobSkill> jobSkills1 = context.JobSkills
                        .Where(js => js.JobId == job.Id)
                        .Include(js => js.Skill)
                        .ToList();

                        JobDetailViewModel newDisplayJob = new JobDetailViewModel(job, jobSkills1);
                        displayJobs.Add(newDisplayJob);
                    }


                }
            }

            ViewBag.columns = ListController.ColumnChoices;
            ViewBag.title = "Jobs with " + ListController.ColumnChoices[searchType] + ": " + searchTerm;
            ViewBag.jobs = displayJobs;
            ViewBag.check = searchType;

            return View("Index");
        }
    }
}
