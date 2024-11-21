using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string teamsWebhookUrl = "";

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


