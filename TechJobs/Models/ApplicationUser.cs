using System;
using Microsoft.AspNetCore.Identity;
using TechJobs.Models;
using System.Collections.Generic;
using TechJobs.Data;
using System.Linq;


namespace TechJobs.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Notify { get; set; }
        public string Location { get; set; }
        
        public List<UserSkill> UserSkills { get; set; }

        public ApplicationUser()
        {
        }

        
    }
}
