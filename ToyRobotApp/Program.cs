using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToyRobot.Models;
using ToyRobot.Services;

namespace ToyRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<ToyRobotApp>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            ToyRobotConfig config = new ToyRobotConfig();
            configuration.Bind("ToyRobotConfig", config);

            services.AddSingleton<ToyRobotConfig>(config);
            services.AddTransient<ToyRobotApp>();
            services.AddTransient<IRobotService, RobotService>();
            services.AddTransient<ICommandParserService, CommandParserService>();

            return services;
        }
    }
}
