using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NCC.Model;

namespace NCC.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmailController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("contactus")]
        public async Task<bool> ContactUs([FromBody] ContactUs requestVM)
        {
            var htmlMessage = $"Name: {requestVM.Name} <br> Email: {requestVM.Email} <br> Phone: {requestVM.PhoneNumber} <br> Address: {requestVM.Address} <br> Organization: {requestVM.Organization} <br> Message: {requestVM.Message}";
            return SendEmailAsync(_config.GetValue<string>("Email:EmailToID"), requestVM.Subject,htmlMessage);
        }

        [HttpPost("myinfo")]
        public async Task<bool> MyInfo([FromBody] MyInfo requestVM)
        {
            var htmlMessage = $"Name: {requestVM.Fname} <br> Email: {requestVM.Email} <br> Phone: {requestVM.PhoneNumber} <br> Account: {requestVM.Account} <br> Hospital: {requestVM.Hospital} <br> <br> Language: {requestVM.Language} <br> Message: {requestVM.Message}";
            return SendEmailAsync(_config.GetValue<string>("Email:EmailToID"), requestVM.Email + "Info", htmlMessage);
        }

        private bool SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
               
                MailMessage message = new MailMessage(_config.GetValue<string>("Email:EmailID"), email, subject, htmlMessage);
                SmtpClient client = new SmtpClient(_config.GetValue<string>("Email:Host"));
                client.Port = 587;
                client.Credentials = new NetworkCredential(_config.GetValue<string>("Email:EmailID"), _config.GetValue<string>("Email:Password"));

                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.EnableSsl = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                message.IsBodyHtml = true;
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
