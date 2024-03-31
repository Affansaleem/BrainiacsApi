using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BrainiacsApi.Data;
using BrainiacsApi.Interface;
using BrainiacsApi.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add DbContext and Identity services.
builder.Services.AddDbContext<BrainiacsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BrainiacsConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BrainiacsDbContext>()
    .AddDefaultTokenProviders(); // Add default token providers


builder.Services.AddTransient<SmtpClient>(provider =>
{
    return new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential("m.affansalim@gmail.com", "frzuljqbytloyqvt"),
        EnableSsl = true,
    };
});

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Add Swagger services.
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
