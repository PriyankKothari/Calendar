using Calender.Business.Models;
using Calender.Business.Services;

namespace Calender.Console
{
    internal sealed class CalenderCommandExecutor
    {
        private readonly IAppointmentService _appointmentService;

        public CalenderCommandExecutor(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        public void ExecuteCommand(string executionCommand, string[] parameters)
        {
            switch (executionCommand)
            {
                case "FIND":
                    if (parameters.Length != 1)
                    {
                        System.Console.WriteLine("Invalid number of arguments for FIND command.");
                        return;
                    }
                    this.FindAvailableTimeSlots(parameters);
                    break;
                case "ADD":
                    if (parameters.Length != 2)
                    {
                        System.Console.WriteLine("Invalid number of arguments for ADD command.");
                        return;
                    }
                    this.AddAppointment(parameters);
                    break;
                case "DELETE":
                    if (parameters.Length != 2)
                    {
                        System.Console.WriteLine("Invalid number of arguments for DELETE command.");
                        return;
                    }
                    this.DeleteAppointment(parameters);
                    break;
                case "KEEP":
                    if (parameters.Length != 1)
                    {
                        System.Console.WriteLine("Invalid number of arguments for KEEP command.");
                        return;
                    }
                    this.KeepAppointment(parameters);
                    break;
                default:
                    System.Console.WriteLine("Invalid command.");
                    break;
            }
        }

        private void FindAvailableTimeSlots(string[] parameters)
        {
            DateTime? date = DateTimeHelper.ParseDateTime(parameters[0]);

            if (date.HasValue)
            {
                List<AppointmentModel> availableTimeSlots = _appointmentService.FindAvailableTimeslots(date.Value, CancellationToken.None).Result;

                if (availableTimeSlots.Any())
                {
                    System.Console.WriteLine($"Available timeslots for {date.Value:dd/mm/yyyy} are: ");
                    availableTimeSlots.ForEach(timeSlot => System.Console.WriteLine($"From: {timeSlot.StartTime:HH:mm} To: {timeSlot.EndTime:HH:mm}"));
                }
                else
                {
                    System.Console.WriteLine($"No timeslots for {date.Value:dd/mm/yyyy} is available.");
                }
            }
            else
            {
                System.Console.WriteLine("Invalid date format.");
            }
        }

        private void AddAppointment(string[] parameters)
        {
            var date = DateTimeHelper.ParseDateTime(parameters[0]);
            var time = DateTimeHelper.ParseTime(parameters[1]);
            
            if (date == null || time == null)
            {
                System.Console.WriteLine("Invalid date or time format.");
                return;
            }

            var startTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Value.Hour, time.Value.Minute, 0);
            var endTime = startTime.AddMinutes(30);

            var appointmentModel = _appointmentService.AddAppointment(new AppointmentModel
            {
                StartTime = startTime,
                EndTime = endTime
            }, CancellationToken.None).Result;

            if (appointmentModel != null)
                System.Console.WriteLine($"Appointment added From: {startTime:dd/MM HH:mm} To: {endTime:dd/MM HH:mm}");
            else
                System.Console.WriteLine($"Appointment cannot be added From: {startTime:dd/MM HH:mm} To: {endTime:dd/MM HH:mm}");
        }

        private void DeleteAppointment(string[] parameters)
        {
            var date = DateTimeHelper.ParseDateTime(parameters[0]);
            var time = DateTimeHelper.ParseTime(parameters[1]);
            
            if (date == null || time == null)
            {
                System.Console.WriteLine("Invalid date or time format.");
                return;
            }

            var startTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Value.Hour, time.Value.Minute, 0);

            bool isDeleted =_appointmentService.DeleteAppointment(startTime, CancellationToken.None).Result;

            if(isDeleted)
                System.Console.WriteLine($"Appointment From: {startTime:dd/MM HH:mm} To: {startTime.AddMinutes(30):dd/MM HH:mm} is deleted");
            else
                System.Console.WriteLine($"Appointment From: {startTime:dd/MM HH:mm} To: {startTime.AddMinutes(30):dd/MM HH:mm} cannot be deleted");
        }

        private void KeepAppointment(string[] parameters)
        {
            var time = DateTimeHelper.ParseTime(parameters[0]);
            
            if (time == null)
            {
                System.Console.WriteLine("Invalid time format.");
                return;
            }

            var startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, time.Value.Hour, time.Value.Minute, 0);
            
            var appointmentModel = _appointmentService.KeepAppointment(startTime, CancellationToken.None).Result;

            System.Console.WriteLine($"Appointment attended successfully at {startTime:HH:mm}");
        }
    }
}
