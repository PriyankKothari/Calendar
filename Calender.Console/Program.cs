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
                .AddTransient<IAppointmentService, AppointmentService>()
                .AddTransient<IAppointmentRepository, AppointmentRepository>()                
                .BuildServiceProvider();

            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: [command] [arguments]");
                return;
            }

            var command = args[0].ToUpper();
            var parameters = args.Skip(1).ToArray();

            CalenderCommandExecutor calenderCommandExecutor = new(serviceProvider?.GetService<IAppointmentService>());
            calenderCommandExecutor.ExecuteCommand(command, parameters);
        }
    }
}