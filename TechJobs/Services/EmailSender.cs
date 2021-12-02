using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using TechJobs.Models;

namespace TechJobs.Services
{
    public class EmailSender : IEmailSender
    {
        private static readonly string s_notifyHTML = System.IO.File.ReadAllText("notify.html");

        private static readonly string s_footer = "<br><br><br><p style='padding: 20px 0px; font-size: 12px; color: #777777; text-align: center;'>&#xA9; Copyright 2021 TechJobs All Rights Reserved.</p>";

        private static readonly string s_logoTag = "<img src='https://i.imgur.com/SJg5nzm.png' style='width: 170px;height: auto; border-radius: 15px;'>";

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("techjobspersistent@gmail.com", "TechJobs"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }

        public Task InitialEmailAsync(ApplicationUser user)
        {
            string bodyHTML = s_logoTag + "</br><h2>Thanks for signing up to receive email notifications!</h2>";
            bodyHTML += s_footer;

            return SendEmailAsync(user.Email, "Welcome, " + user.FirstName + "!", bodyHTML);

        }

        public Task LocationEmailAsync(List<Job> userJobs, string email)
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

            return SendEmailAsync(email, "Jobs in your location!", bodyHTML);
        }

        public Task NotifyAsync(ApplicationUser user, Job j)
        {
            string bodyHTML = s_notifyHTML.Replace("{0}", user.FirstName);
            bodyHTML = bodyHTML.Replace("{1}", j.Name);
            bodyHTML = bodyHTML.Replace("{2}", "<a href='" + j.Employer.Url + "'>" + j.Employer.Name + "</a>");
            bodyHTML = bodyHTML.Replace("{3}", j.Employer.Location);

            return SendEmailAsync(user.Email, "Hey " + user.FirstName + ", new job posting!", bodyHTML);
            
        }

    }
}