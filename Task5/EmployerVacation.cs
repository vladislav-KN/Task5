namespace Task5;

public class EmployerVacation
{
    private string name;
    private List<VacationRange> vacationList;
    public EmployerVacation(string name)
    {
        this.name = name;
        this.vacationList = new List<VacationRange>();
    }
     

    public string Name => name;
    public List<VacationRange> VacationList
    {
        get
        {
            return vacationList;
        }
        set
        {
            vacationList = value;
        }
    }

    public void CreateVacationList(int range, int year,  ref List<int[]> alreadyPlanedVacationIntervals, int maxDays = 28)
    {
        //проверка кратности maxDays недели
        if(maxDays%7!=0)
            return;
        Random rnd = new Random();
        //создаем список отпусков
        while (maxDays > 0)
        {
            int daysOfVacation;
            //
            if (maxDays==7)
            {
                daysOfVacation = 7;
            }
            else
            {
                daysOfVacation = rnd.Next(0, 2) * 7 + 7;
            }

            CreateVacation(range,daysOfVacation,year,ref alreadyPlanedVacationIntervals);
            maxDays -= daysOfVacation;

        }
    }
    public void CreateVacation(int rangeMax, int daysOfVacation, int year, ref List<int[]> intervals)
    {
        Random rnd = new Random();
        int randomNumber;
        DateTime startingDateTime = new DateTime(year, 1, 1);
        while (true)
        {
            randomNumber = rnd.Next(0, rangeMax + 1 - daysOfVacation);
            DateTime startOfVacation = startingDateTime.AddDays(randomNumber);
            DateTime endOfDateTime = startingDateTime.AddDays(randomNumber+daysOfVacation);
            //смотрим попадают ли даты в хотябы 1 промежуток общий или промежуток класса
            bool isDateInAllIntervals = intervals.Any( interval=>
                randomNumber >= interval[0]-3 && randomNumber <= interval[1]+3 &&
                randomNumber+daysOfVacation  >= interval[0]-3 && randomNumber <= interval[1]+3
            );
            //исключение случаев когда существует отпуск который был в прошлом текущем и следующем месяце из выбранного работником
            bool isDateInEmployerIntervals = vacationList.Any(
                vacation => 
                    (
                        vacation.StartVDateTime?.Month == startOfVacation.Month 
                        || vacation.StartVDateTime?.Month == startOfVacation.AddMonths(1).Month
                        ||  vacation.StartVDateTime?.Month == startOfVacation.AddMonths(-1).Month
                    )
                    ||
                    (
                        vacation.StartVDateTime?.Month == endOfDateTime.Month 
                        || vacation.StartVDateTime?.Month == endOfDateTime.AddMonths(1).Month 
                        ||  vacation.StartVDateTime?.Month == endOfDateTime.AddMonths(-1).Month
                    )
                    ||
                    (
                        vacation.EndVDateTime?.Month == startOfVacation.Month 
                        || vacation.EndVDateTime?.Month == startOfVacation.AddMonths(1).Month
                        ||  vacation.EndVDateTime?.Month == startOfVacation.AddMonths(-1).Month
                    )
                    ||
                    (
                        vacation.EndVDateTime?.Month == endOfDateTime.Month 
                        || vacation.EndVDateTime?.Month == endOfDateTime.AddMonths(1).Month 
                        ||  vacation.EndVDateTime?.Month == endOfDateTime.AddMonths(-1).Month
                    )
            );
            //если нашли место для отпуска сохраняем и завершаем работу функции
            if (!isDateInAllIntervals && !isDateInEmployerIntervals)
            {
                intervals.Add([randomNumber,randomNumber+daysOfVacation]);
                vacationList.Add(new VacationRange(startOfVacation,endOfDateTime));
                return;
            }
        }
        
         
    }
}

public class VacationRange:IComparable
{
    private DateTime? startVDateTime;
    private DateTime? endVDateTime;

    public VacationRange(DateTime startVDateTime, DateTime endVDateTime)
    {
        this.endVDateTime = endVDateTime;
        this.startVDateTime = startVDateTime;
    }

    public DateTime? StartVDateTime => startVDateTime;
    public DateTime? EndVDateTime => endVDateTime;
    
    //вычисляем период отпуска без учёта выходных
    public void CalculateEndOfVacation(ref int vacationDays)
    {
        if(startVDateTime is null)
            return;
        int daysRemaining = vacationDays;
        int startDayOfWeak = (int)startVDateTime?.DayOfWeek!;
        //делаем вычисляем на первую неделю
        switch (startDayOfWeak)
        {
            case 0:
                vacationDays += 1;
                break;
            case 6:
                vacationDays += 2;
                break;
            default:
                daysRemaining -= 5 - startDayOfWeak + 1;
                vacationDays += 2;
                break;
        }
        //пока у нас есть возможность отнять 5 дней добавлеяем 2 выходных к итоговой сумме дней
        while (daysRemaining>5)
        {
            daysRemaining -= 5;
            vacationDays += 2;
        }
        //вычисляем полное количество дней
        endVDateTime = startVDateTime?.AddDays(vacationDays);
        
    }
    //сравнение для сортировки по дате или по такому же классу
    public int CompareTo(object? obj)
    {
        if (startVDateTime == null || obj == null) return -1;
        DateTime? comparableDateTime;
        if (typeof(VacationRange) == obj?.GetType())
        {
            comparableDateTime = ((VacationRange)obj).startVDateTime;
        }
        else if (typeof(DateTime) == obj?.GetType())
        {
            comparableDateTime= (DateTime)obj;
        }
        else
        {
            return -1;
        }

        if (comparableDateTime != null)
            return startVDateTime.Value.CompareTo(comparableDateTime.Value);
        else
            return -1;
    }
}
