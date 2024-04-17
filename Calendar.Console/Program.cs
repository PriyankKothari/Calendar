// See https://aka.ms/new-console-template for more information
using Calender.Business.Services;
using Calender.Persistent.DatabaseContexts;
using Calender.Persistent.Repositories;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddDbContext<DatabaseContext>()
    .AddTransient<IAppointmentRepository, AppointmentRepository>()
    .AddTransient<IAppointmentService, AppointmentService>()
    .BuildServiceProvider();
