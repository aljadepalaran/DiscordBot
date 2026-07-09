using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
  .AddDiscordGateway(options =>
    {
        options.Token = builder.Configuration["Discord:Token"]!;
    })
    .AddApplicationCommands();

var host = builder.Build();


host.AddSlashCommand("ping", "Ping!", (int number) =>
  {
    var output = new List<string>();
    for (int i = 0; i < number; i++)
    {
        output.Add("Pong!");
    }
    return string.Join("", output);
  });
host.AddUserCommand("Username", (User user) => user.Username);
host.AddMessageCommand("Length", (RestMessage message) => message.Content.Length.ToString());

// Add commands from modules
// host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
