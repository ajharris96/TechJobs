using System;
using System.Net.Mail;
using System.Net;
using TechJobs.Models;
using TechJobs.ViewModels;
using Microsoft.AspNetCore.Identity;
using TechJobs.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TechJobs.Services
{
    public class Emailer
    {
        private static SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("techjobspersistent@gmail.com", "LaunchCode75?"),
            EnableSsl = true,
        };

        

        public Emailer()
        {
        }


        public static void Notify(List<ApplicationUser> users, Job j)
        {
            foreach (ApplicationUser u in users)
            {
                if (u.Notify)
                {
                    string bodyHTML = "<h3>A new job for you was just posted, " + u.FirstName + "!</h3>";
                    bodyHTML += "<p>" + j.Name + ", " + j.Employer.Name + ", " + j.Employer.Location + "</p>";
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("techjobspersistent@gmail.com"),
                        Subject = "Hey " + u.FirstName + ", new job posting!",
                        Body = bodyHTML,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(u.Email);

                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}
