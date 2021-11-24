using System;
namespace TechJobs.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public Job Job { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public JobSkill()
        {
        }

        public JobSkill(int jobId, int skillId)
        {
            JobId = jobId;
            SkillId = skillId;
        }
    }
}