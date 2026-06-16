using OpenTrainingPlatform.Infrastructure;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    //app.UseHttpsRedirection();

    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while starting the application: {ex.Message}");
}
finally
{
    Console.WriteLine("Application has stopped.");
}
