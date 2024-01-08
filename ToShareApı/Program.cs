using Microsoft.EntityFrameworkCore;
using System.Threading;
using ToShareApý.Data;
using ToShareApý.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionstrings = builder.Configuration.GetConnectionString("TutorialDbConnection");
builder.Services.AddDbContext<ApiDbContext>(
    option => option.UseSqlServer(connectionstrings));

builder.Services.AddSingleton<ApplyApprovalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Zamanlayýcý iþlemini baþlat
StartTimer();

app.Run();

void StartTimer()
{
    var serviceProvider = app.Services;
    var timerInterval = TimeSpan.FromMinutes(1); // 15 dakikada bir kontrol et, bu süreyi ihtiyacýnýza göre ayarlayabilirsiniz

    var applyApprovalService = serviceProvider.GetRequiredService<ApplyApprovalService>();
    TimerCallback timerCallback = new TimerCallback(state =>
    {
        // Timer her çalýþtýðýnda ApplyApprovalService'yi çaðýr
        applyApprovalService.CheckPostApplies(state);
    });

    Timer timer = new Timer(timerCallback, null, TimeSpan.Zero, timerInterval);
}