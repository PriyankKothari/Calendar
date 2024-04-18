# Calendar
Caldner is a console application that accepts FIND, ADD, DELETE and KEEP commands with appropriate Date {DD/MM} and/or Time {HH:MM} format.

## Source Code
[GitHub](https://github.com/PriyankKothari/Calendar)

## Getting Started
- Clone the repository:
git clone https://github.com/PriyankKothari/Calender.git
  - Visual Studio
    - Open solution (Calender.sln) in Visual Studio
    - Build solution
    - Right click Calender.Console project, click on properties, go to Debug, and open Debug Launch profile.
    - Set command + parameters as below to execute:
      - FIND {DD/MM} e.g. FIND 12/04
      - ADD {DD/MM} {HH:MM} e.g. ADD 12/04 15:00
      - DELETE {DD/MM} {HH:MM} e.g. DELETE 12/04 15:00
      - KEEP {HH:MM} e.g. KEEP 15:00
  - Command Prompt
    - Run command prompt as Administrator
    - Navigate to the bin folder of the Calender.Console project
    - Navigate to bin > Debug > net6.0 folder
    - Enter Calender.Console.exe COMMAND PARAMETERS as below to execute:
      - Calender.Console.exe FIND {DD/MM} e.g. FIND 12/04
      - Calender.Console.exe ADD {DD/MM} {HH:MM} e.g. ADD 12/04 15:00
      - Calender.Console.exe DELETE {DD/MM} {HH:MM} e.g. DELETE 12/04 15:00
      - Calender.Console.exe KEEP {HH:MM} e.g. KEEP 15:00
## Imrpovements / TODO
- Implement business validations such as working hours and reserved hours using an attribute to avoid writing validation code in business service
- Implement API with endpoints and use endpoints from the Console app. This way, the business validations such as working hours and reserved hours can be applied earlier at the API level.
- Configurable Working Hours and Reserved Hours
- Configurable database connection string
- Logging
- Avoid exceptions. Log errors and continue with user friendly message instead of crashing the application.
- More unit tests to cover more scenarios.
- Helper methods to set acceptable and reserved dates in the future to make the tests run anytime without "date" dependency
