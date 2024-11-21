using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string teamsWebhookUrl = "https://algomabrampton.webhook.office.com/webhookb2/deeb000e-0923-4759-9fe0-cdaf8bf0c0c8@0120dad2-7095-421b-9677-8b7ee0c9124e/IncomingWebhook/256f95b32f8e47b5bd38407bacd6a85b/071ed9f8-23a3-4268-919b-804b7fcdd513/V2ucfS6NSc5UfVd6w-QCRpEPuFxWK7HmCBMODg6t8lcpc1";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("D://Logger/ExceptionHandlingSerilog.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MicrosoftTeams(
                teamsWebhookUrl,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, // Send only Error logs or higher to Teams
                title: "🚨 Application Error Notification 🚨" // Custom title for Teams messages
            )
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Application started");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    app.Use(async (context, next) =>
    {
        try
        {
            await next(); // Pass control to the next middleware
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = 500; // Internal Server Error
            await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
        }
    });

}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}


