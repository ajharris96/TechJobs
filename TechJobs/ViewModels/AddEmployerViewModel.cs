using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechJobs.Models;

namespace TechJobs.ViewModels
{
    public class AddEmployerViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required")]

        public string Location { get; set; }

        [Required(ErrorMessage ="Url is required.")]
        public string Url { get; set; }

        public AddEmployerViewModel()
        {
        }
    }
}
