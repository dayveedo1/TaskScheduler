using Hangfire;
using HangfireTest;
using HangfireTest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<HangFireTestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HangfireCon"));
});

builder.Services.AddHangfire(options =>
{
    options.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireCon").ToString());
});

builder.Services.AddHangfireServer();

var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfig>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddScoped<IEmailSender, IEmailSenderImpl>();

string AllowOrigins = builder.Configuration["Cors"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigins",
               builder =>
               {
                   builder.SetIsOriginAllowed(origin => true)
                //builder.WithOrigins(AllowOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            
        });
});

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    ////app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(AllowOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/dashboard");

//app.UseHangfireServer();

app.MapControllers();

app.Run();
