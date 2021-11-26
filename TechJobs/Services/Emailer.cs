﻿using System;
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

        private static string NotifyHtml = System.IO.File.ReadAllText("index2.html");



        public static void Notify(List<ApplicationUser> users, Job j)
        {
            foreach (ApplicationUser u in users)
            {
                if (u.Notify)
                {
                    //string bodyHTML = "<h3>A new job for you was just posted, " + u.FirstName + "!</h3>";
                    //bodyHTML += "<p>" + j.Name + ", " + j.Employer.Name + ", " + j.Employer.Location + "</p>";

                    //string body = String.Format(NotifyHtml, u.FirstName, j.Name, j.Employer.Name, j.Employer.Location);

                    string body = NotifyHtml.Replace("{0}", u.FirstName);
                    body = body.Replace("{1}", j.Name);
                    body = body.Replace("{2}", j.Employer.Name);
                    body = body.Replace("{3}", j.Employer.Location);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("techjobspersistent@gmail.com"),
                        Subject = "Hey " + u.FirstName + ", new job posting!",
                        Body = body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(u.Email);

                    smtpClient.Send(mailMessage);
                }
            }
        }

        public static void LocationEmail(string bodyHTML, ApplicationUser user)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com"),
                Subject = "Jobs in your location!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            smtpClient.Send(mailMessage);
        }

        public static void InitialEmail(ApplicationUser user)
        {
            string bodyHTML = "<h2>Thanks for signing up to receive email notification!</h2>";

            

            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com"),
                Subject = "Welcome, " + user.FirstName + "!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            smtpClient.Send(mailMessage);
        }

    }
}