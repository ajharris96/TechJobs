using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechJobs.Models;
using TechJobs.Data;
using Microsoft.EntityFrameworkCore;

namespace TechJobs.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ApplicationDbContext context;
        public List<string> locations;
        public List<UserSkill> skills;
        public List<Skill> PossibleSkills;
        public string MySkills;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            context = dbcontext;
            locations = context.Employers.Select(e => e.Location).Distinct().OrderBy(x => x).ToList();
            PossibleSkills = context.Skills.ToList();

        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            [Required]
            [Display(Name = "Location")]
            public string Location { get; set; }

            [Display(Name = "Notify me of new job postings")]
            public bool Notify { get; set; }

            public int SkillId { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
                
            };

            ApplicationUser user1 = context.Users.Where(u => u.UserName == userName).ToList()[0];


            List<UserSkill> userSkills = context.UserSkills
                .Where(u => u.UserId == user1.Id)
                .Include(u => u.Skill)
                .ToList();

            skills = userSkills;

            foreach (var s in userSkills)
            {
                var index = PossibleSkills.FindIndex(x => x.Id == s.SkillId);
                PossibleSkills.Remove(PossibleSkills[index]);
            }


            




            MySkills = "My Skills: ";
            foreach (var s in skills)
            {
                MySkills += s.Skill.Name + ", ";
            }
            MySkills = MySkills.Trim();
            MySkills = MySkills.TrimEnd(',');


        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var userName = await _userManager.GetUserNameAsync(user);

            ApplicationUser user1 = context.Users.Where(u => u.UserName == userName).ToList()[0];

            user1.Location = Input.Location;
            user1.Notify = Input.Notify;

            context.Users.Update(user1);
            context.SaveChanges();


            if (Input.SkillId != 0)
            {
                UserSkill newSkill = new UserSkill(user.Id, Input.SkillId);

                context.UserSkills.Add(newSkill);

                context.SaveChanges();

            }
            

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
