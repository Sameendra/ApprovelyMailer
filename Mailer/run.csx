#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Net.Mail;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    // testing github integration

    string jsonContent = await req.Content.ReadAsStringAsync();

    log.Info(jsonContent);

    dynamic data = JsonConvert.DeserializeObject(jsonContent);


    // if (data.first == null || data.last == null) {
    //     return req.CreateResponse(HttpStatusCode.BadRequest, new {
    //         error = "Please pass first/last properties in the input object"
    //     });
    // }

    bool isImportantEmail = bool.Parse(data.isImportant.ToString());
    string fromEmail = data.fromEmail;
    string toEmail = data.toEmail;
    int smtpPort = 587;
    bool smtpEnableSsl = true;
    string smtpHost = "smtp.gmail.com"; // your smtp host
    string smtpUser = "sameendra.office@gmail.com"; // your smtp user
    string smtpPass = "Sameendra1992"; // your smtp password
    string subject = data.subject;
    string message = data.message;

    log.Info(fromEmail);
    
    MailMessage mail = new MailMessage();
    mail.From = new System.Net.Mail.MailAddress(smtpUser);
    SmtpClient client = new SmtpClient();
    client.Port = smtpPort;
    client.EnableSsl = smtpEnableSsl;
    client.DeliveryMethod = SmtpDeliveryMethod.Network;
    client.UseDefaultCredentials = true;
    client.Host = smtpHost;
    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
    mail.Subject = subject;
    mail.To.Add(new MailAddress(toEmail));
    
    if (isImportantEmail) {
      mail.Priority = MailPriority.High;
    }
    
    mail.Body = message;
    try {
      client.Send(mail);
      log.Verbose("Email sent.");
      return req.CreateResponse(HttpStatusCode.OK, new {
            status = true,
            message = string.Empty
        });
    }
    catch (Exception ex) {
      log.Info(ex.ToString());
      return req.CreateResponse(HttpStatusCode.InternalServerError, new {
            status = false,
            message = "Message has not been sent. Check Azure Function Logs for more information."
        });
    }

    // return req.CreateResponse(HttpStatusCode.OK, new {
    //     greeting = $"Hello {data.first} {data.last}!"
    // });
}
