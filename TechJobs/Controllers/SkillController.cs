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
    [Authorize]
    public class SkillController : Controller
    {
        private ApplicationDbContext context;

        public SkillController(ApplicationDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Skill> skills = context.Skills.ToList();
            return View(skills);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Add()
        {
            Skill skill = new Skill();
            return View(skill);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Add(Skill skill)
        {
            if (ModelState.IsValid)
            {
                context.Skills.Add(skill);
                context.SaveChanges();
                return Redirect("/Skill/");
            }

            return View("Add", skill);
        }

        [Authorize(Roles = "manager")]
        public IActionResult AddJob(int id)
        {
            Job theJob = context.Jobs.Find(id);
            List<Skill> possibleSkills = context.Skills.ToList();
            AddJobSkillViewModel viewModel = new AddJobSkillViewModel(theJob, possibleSkills);
            return View(viewModel);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult AddJob(AddJobSkillViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                int jobId = viewModel.JobId;
                int skillId = viewModel.SkillId;

                List<JobSkill> existingItems = context.JobSkills
                    .Where(js => js.JobId == jobId)
                    .Where(js => js.SkillId == skillId)
                    .ToList();

                if (existingItems.Count == 0)
                {
                    JobSkill jobSkill = new JobSkill
                    {
                        JobId = jobId,
                        SkillId = skillId
                    };
                    context.JobSkills.Add(jobSkill);
                    context.SaveChanges();
                }

                return Redirect("/Home/Detail/" + jobId);
            }

            return View(viewModel);
        }

        public IActionResult About(int id)
        {
            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.SkillId == id)
                .Include(js => js.Job)
                .Include(js => js.Skill)
                .ToList();

            return View(jobSkills);
        }

    }
}
