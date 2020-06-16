using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sors.Authentication;
using sors.Data;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using sors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly string _from;
        private readonly string _siteAddress;
        private readonly IUserService _userService;

        public MailController(DataContext context, IConfiguration configuration, IUserService userService)
        {
            _from = configuration.GetValue<string>("Email:From");
            _siteAddress = configuration.GetValue<string>("SiteAddress");
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(SorsMail sorsMessage)
        {
            try
            {
                using (var smtpClient = HttpContext.RequestServices.GetRequiredService<SmtpClient>())
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_from);
                    foreach (var recipient in sorsMessage.To)
                    {
                        //mailMessage.To.Add(new MailAddress(_userService.GetEmail(recipient)));
                        mailMessage.To.Add(new MailAddress(recipient));
                    }
                    mailMessage.IsBodyHtml = true;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Subject = sorsMessage.Subject;
                    mailMessage.Body = GenerateBody(sorsMessage.Text, sorsMessage.Link, sorsMessage.Comment);

                    await smtpClient.SendMailAsync(mailMessage);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send message, " + ex.Message);
            }
        }

        private string GenerateBody(string text, string link, string comment)
        {
            string body = "<html>" +
                          "<h2>Система обработки рисковых событий</h2>" +
                          "<p>" + text + "</p>" +
                          "<p>" + _siteAddress + link + "</p>" +
                          "<p>" + comment + "</p>" +
                          "</html>";
            return body;
        }

    }
}
