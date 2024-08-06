using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleEmailServiceV2>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/send-mail", async (
    [FromServices] IAmazonSimpleEmailServiceV2 _amazonSimpleEmailService,
    [FromBody] MailDto mailDto) =>
{
    var charSet = "UTF-8";

    var sendEmailRequest = new SendEmailRequest()
    {
        FromEmailAddress = mailDto.FromEmail,
        Destination = new Destination()
        {
            ToAddresses = mailDto.toEmail.ToList()
        },
        Content = new EmailContent()
        {
            Simple = new Message()
            {
                Subject = new Content()
                {
                    Data = mailDto.subject,
                    Charset = charSet
                },
                Body = new Body()
                {
                    Html = new Content()
                    {
                        Data = mailDto.htmlContent,
                        Charset = charSet
                    },
                    Text = new Content()
                    {
                        Data = mailDto.textContent,
                        Charset = charSet
                    }
                }
            }
        }


    };

    var sendEmailResponse = await _amazonSimpleEmailService.SendEmailAsync(sendEmailRequest);


    return Results.Ok(sendEmailResponse);
});

app.MapGet("get-account-details", async (
     [FromServices] IAmazonSimpleEmailServiceV2 _amazonSimpleEmailService) =>
{

    var getAccountRequest = new GetAccountRequest()
    {
    };

    var getAccountResponse = await _amazonSimpleEmailService.GetAccountAsync(getAccountRequest);


    return Results.Ok(getAccountResponse);
});


app.Run();

internal record MailDto(
    IEnumerable<string> toEmail,
    string FromEmail,
    string subject,
    string textContent,
    string htmlContent);
