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
        private static readonly SmtpClient s_smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("techjobspersistent@gmail.com", "LaunchCode75?"),
            EnableSsl = true,
        };

        private static readonly string s_notifyHTML = System.IO.File.ReadAllText("notify.html");

        private static readonly string s_footer = "<br><br><br><p style='padding: 20px 0px; font-size: 12px; color: #777777; text-align: center;'>&#xA9; Copyright 2021 TechJobs All Rights Reserved.</p>";

        private static readonly string s_logoTag = "<img src='https://i.imgur.com/SJg5nzm.png' style='width: 170px;height: auto; border-radius: 15px;'>";




        public static void Notify(List<ApplicationUser> users, Job j)
        {
            foreach (ApplicationUser u in users)
            {
                if (u.Notify)
                {
                    

                    string bodyHTML = s_notifyHTML.Replace("{0}", u.FirstName);
                    bodyHTML = bodyHTML.Replace("{1}", j.Name);
                    bodyHTML = bodyHTML.Replace("{2}", "<a href='"+j.Employer.Url+"'>" + j.Employer.Name +"</a>");
                    bodyHTML = bodyHTML.Replace("{3}", j.Employer.Location);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("techjobspersistent@gmail.com","TechJobs"),
                        Subject = "Hey " + u.FirstName + ", new job posting!",
                        Body = bodyHTML,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(u.Email);

                    s_smtpClient.Send(mailMessage);
                }
            }
        }

        public static void InitialEmail(ApplicationUser user)
        {
            string bodyHTML = s_logoTag + "</br><h2>Thanks for signing up to receive email notifications!</h2>";
            bodyHTML += s_footer;
            

            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com","TechJobs"),
                Subject = "Welcome, " + user.FirstName + "!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            s_smtpClient.Send(mailMessage);
        }

        public static void LocationEmail(List<Job> userJobs, string email)
        {
            string bodyHTML = s_logoTag + "</br>";

            if (userJobs.Count == 0)
            {
                bodyHTML += "<h2>There are no jobs currently available in your city!</h1></br><h3>Check back soon!</h3></br>";
                bodyHTML += s_footer;
            }
            else
            {
                bodyHTML += "<h2>Here are the best job opportunities available to you!</h1></br><ul>";

                for (int i = 0; i < userJobs.Count; i++)
                {
                    bodyHTML += "<li>" + userJobs[i].Name + ", " + "<a href='" + userJobs[i].Employer.Url + "'>" + userJobs[i].Employer.Name + "</a>" + ", " + userJobs[i].Employer.Location + "</li></br>";
                }

                bodyHTML += "</ul>";
                bodyHTML += s_footer;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress("techjobspersistent@gmail.com", "TechJobs"),
                Subject = "Jobs in your location!",
                Body = bodyHTML,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            s_smtpClient.Send(mailMessage);



        }

    }
}
