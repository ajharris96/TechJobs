using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechJobs.Models;
using TechJobs.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TechJobs.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace TechJobs.ViewModels
{
    public class AddJobViewModel
    {
        
        public string Name { get; set; }
        
        public int EmployerId { get; set; }

        public List<Employer> SelectListItem { get; set; }

        public List<Skill> Skills { get; set; }

        public List<Skill> PossibleSkills { get; set; }

        public AddJobViewModel()
        {
            
        }

        
    }
}
