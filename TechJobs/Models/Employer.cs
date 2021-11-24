using System;
namespace TechJobs.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }

        public Employer()
        {
        }

        public Employer(string name, string location, string url)
        {
            Name = name;
            Location = location;
            Url = url;
        }
    }
}
