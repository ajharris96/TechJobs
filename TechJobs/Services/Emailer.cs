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

        private static string NotifyHtml = System.IO.File.ReadAllText("notify.html");
        public static string footer = "<br><br><br><p style='padding: 20px 0px; font-size: 12px; color: #777777; text-align: center;'>&#xA9; Copyright 2021 TechJobs All Rights Reserved.</p>";



        public static void Notify(List<ApplicationUser> users, Job j)
        {
            foreach (ApplicationUser u in users)
            {
                if (u.Notify)
                {
                    

                    string body = NotifyHtml.Replace("{0}", u.FirstName);
                    body = body.Replace("{1}", j.Name);
                    body = body.Replace("{2}", "<a href='"+j.Employer.Url+"'>" + j.Employer.Name +"</a>");
                    body = body.Replace("{3}", j.Employer.Location);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("techjobspersistent@gmail.com","TechJobs"),
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
            bodyHTML += footer;

            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com","TechJobs"),
                Subject = "Jobs in your location!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            smtpClient.Send(mailMessage);
        }

        public static void InitialEmail(ApplicationUser user)
        {
            string bodyHTML = "<img src='https://i.imgur.com/SJg5nzm.png' style='width: 170px;height: auto; border-radius: 15px;'><h2>Thanks for signing up to receive email notifications!</h2>";
            bodyHTML += footer;
            

            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com","TechJobs"),
                Subject = "Welcome, " + user.FirstName + "!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            smtpClient.Send(mailMessage);
        }

    }
}
