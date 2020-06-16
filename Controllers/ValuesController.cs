using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Post()
        {
            using (var smtpClient = HttpContext.RequestServices.GetRequiredService<SmtpClient>())
            {
                await smtpClient.SendMailAsync(new MailMessage(
                        "СОРС",
                        "lostuser@lost.net",
                        "first mail",
                        "first bodY"
                    ));

                return Ok("OK");
            }
        }

        [HttpGet("{id}")]
        public ActionResult Post(int id)
        {
            SmtpClient smtpClient = new SmtpClient("exchange.lost.net", 25);
            MailMessage message = new MailMessage();
            try
            {
                MailAddress fromAddress = new MailAddress("lostuser@lost.net", "From Me");
                MailAddress toAddress = new MailAddress("lostuser@lost.net", "To You");
                message.From = fromAddress;
                message.To.Add(toAddress);
                message.Subject = "Testing!";
                message.Body = "This is the body of a sample message -> " + id;
                smtpClient.UseDefaultCredentials = true;
                System.Net.NetworkCredential nc = CredentialCache.DefaultNetworkCredentials;
                smtpClient.Credentials = (System.Net.ICredentialsByHost)nc.GetCredential("exchange.lost.net", 25, "Basic");
                smtpClient.Send(message);
                return Ok("Email sent.");
            }
            catch (Exception ex)
            {
                return BadRequest("Coudn't send the message!\n  " + ex.Message);
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("check")]
        public ActionResult<string> Check()
        {
            var answer = "require user";
            return answer;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
