using DefaultCorsPolicyNugetPackage;
using eHospitalServer.Business;
using eHospitalServer.DataAccess;
using eHospitalServer.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultCors();

builder.Services.AddBusiness();
builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();


ExtensionsMiddleware.CreateFirstUser(app);


app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        await Console.Out.WriteLineAsync(ex.Message);
        throw;
    }
});

app.UseHttpsRedirection();

app.MapControllers()
    .RequireAuthorization(policy =>
    {
        policy.RequireClaim(ClaimTypes.NameIdentifier);
        policy.AddAuthenticationSchemes("Bearer");
    });

app.Run();
