using Calender.Business.Services;
using Calender.Persistent.DatabaseContexts;
using Calender.Persistent.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Calender.Console
{
    internal class Program
    {
        const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Appointments;Integrated Security=True;";

        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DatabaseContext>(options => { options.UseSqlServer(connectionString); })
                .AddTransient<DatabaseContext>()
                .AddTransient<IAppointmentRepository, AppointmentRepository>()
                .AddTransient<IAppointmentService>(provider =>
                {
                    return new AppointmentService(
                        provider.GetRequiredService<IAppointmentRepository>(),
                        new TimeSpan(09, 00, 00),
                        new TimeSpan(17, 00, 00),
                        new TimeSpan(16, 00, 00),
                        new TimeSpan(17, 00, 00));
                })
                .BuildServiceProvider();

            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: [command] [arguments]");
                return;
            }

            var command = args[0].ToUpper();
            var parameters = args.Skip(1).ToArray();

            CommandExecutor commandExecutor = new(serviceProvider?.GetService<IAppointmentService>());
            commandExecutor.ExecuteCommand(command, parameters);
        }
    }
}