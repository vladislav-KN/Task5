using System;
using System.Collections.Generic;
using System.Linq;
using Task5;


List<EmployerVacation> employerVacations = new List<EmployerVacation> ()
{
    new EmployerVacation("Иванов Иван Иванович"),
    new EmployerVacation("Петров Петр Петрович"),
    new EmployerVacation("Юлина Юлия Юлиановна"),
    new EmployerVacation("Сидоров Сидор Сидорович"),
    new EmployerVacation("Павлов Павел Павлович"),
    new EmployerVacation("Георгиев Георг Георгиевич")
};
List<int[]> vacationIntervals = new List<int[]>();
DateTime currentDateTime = DateTime.Now;
DateTime startOfTheYear = new DateTime(currentDateTime.Year, 1, 1);
DateTime endOfTheYear = new DateTime(currentDateTime.Year, 12, 31);
foreach (EmployerVacation employer in employerVacations)
{
    
    employer.CreateVacationList((endOfTheYear - startOfTheYear).Days, currentDateTime.Year, ref vacationIntervals);
    employer.VacationList.Sort();
 
    Console.WriteLine($"________{employer.Name}________");
    
    foreach (VacationRange vacation in employer.VacationList)
    {
        Console.WriteLine($"{vacation.StartVDateTime?.ToShortDateString()}-{vacation.EndVDateTime?.ToShortDateString()}");
        
    }
 
}