using BlazorApp12.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjMzOTE4MUAzMjMxMmUzMDJlMzBaMjl1b2lTVmN1cFdJWG84OCtOUmpOTzNla21XZ2FSVFNMWENrTkhwTjdNPQ==");

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(options =>
        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(options =>
        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
});

builder.Services.AddTransient<CertificateValidationService>();

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate(options =>
        {
            // Configuración del esquema de autenticación por certificado
            options.RevocationMode = X509RevocationMode.NoCheck;
            options.AllowedCertificateTypes = CertificateTypes.All;
            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetService<CertificateValidationService>();
                    if (validationService != null && validationService.ValidateCertificate(context.ClientCertificate))
                    {
                        Console.WriteLine("Success");
                        context.Fail("invalid cert");
                    }
                    else
                    {
                        Console.WriteLine("invalid cert");
                        context.Fail("invalid cert");
                    }
                    return Task.CompletedTask;
                }
            };
        });


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapControllers();

app.Run();


public class CertificateValidationService
{
    public bool ValidateCertificate(X509Certificate2 clientCertificate)
    {

        var cert = new X509Certificate2(Path.Combine("C:\\Users\\User\\Desktop\\certificate123.pfx"), "passwordOfCertificate");
        if (clientCertificate.Thumbprint == cert.Thumbprint)
        {
            return true;
        }
        return false;
    }

}