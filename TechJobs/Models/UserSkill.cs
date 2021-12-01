using System;
namespace TechJobs.Models
{
    public class UserSkill
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public UserSkill()
        {
        }

        public UserSkill(string userId, int skillId)
        {
            UserId = userId;
            SkillId = skillId;
        }
    }
}