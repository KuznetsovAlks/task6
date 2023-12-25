using System;
using System.IO;

public struct Worker
{
    public int Id { get; set; }
    public DateTime AddedDateTime { get; set; }
    public string FIO { get; set; }
    public int Age { get; set; }
    public int Height { get; set; }
    public DateTime BirthDate { get; set; }
    public string BirthPlace { get; set; }
}

public class Repository
{
    private string filePath;

    public Repository(string filePath)
    {
        this.filePath = filePath;
    }

    public Worker[] GetAllWorkers()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            Worker[] workers = new Worker[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split('#');
                workers[i] = new Worker
                {
                    Id = int.Parse(data[0]),
                    AddedDateTime = DateTime.ParseExact(data[1], "dd.MM.yyyy HH:mm", null),
                    FIO = data[2],
                    Age = int.Parse(data[3]),
                    Height = int.Parse(data[4]),
                    BirthDate = DateTime.ParseExact(data[5], "dd.MM.yyyy", null),
                    BirthPlace = data[6]
                };
            }

            return workers;
        }

        return null;
    }

    public void AddWorker(Worker worker)
    {
        worker.Id = GetNextID();
        worker.AddedDateTime = DateTime.Now;

        string newRecord = $"{worker.Id}#{worker.AddedDateTime:dd.MM.yyyy HH:mm}#{worker.FIO}#{worker.Age}#{worker.Height}#{worker.BirthDate:dd.MM.yyyy}#{worker.BirthPlace}";

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, newRecord);
        }
        else
        {
            File.AppendAllText(filePath, "\n" + newRecord);
        }

        Console.WriteLine("Запись успешно добавлена");
    }

    public void DeleteWorker(int id)
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (string line in lines)
                {
                    string[] data = line.Split('#');
                    if (int.Parse(data[0]) != id)
                    {
                        sw.WriteLine(line);
                    }
                }
            }

            Console.WriteLine($"Запись с ID {id} успешно удалена");
        }
        else
        {
            Console.WriteLine("Файл не существует");
        }
    }

    private int GetNextID()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length > 0)
            {
                string lastLine = lines[lines.Length - 1];
                string[] data = lastLine.Split('#');
                if (int.TryParse(data[0], out int lastID))
                {
                    return lastID + 1;
                }
            }
        }
        return 1;
    }

    public Worker[] GetWorkersBetweenTwoDates(DateTime dateFrom, DateTime dateTo)
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            Worker[] workers = new Worker[lines.Length];
            int count = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split('#');
                DateTime addedDateTime = DateTime.ParseExact(data[1], "dd.MM.yyyy HH:mm", null);

                if (addedDateTime >= dateFrom && addedDateTime <= dateTo)
                {
                    workers[count] = new Worker
                    {
                        Id = int.Parse(data[0]),
                        AddedDateTime = addedDateTime,
                        FIO = data[2],
                        Age = int.Parse(data[3]),
                        Height = int.Parse(data[4]),
                        BirthDate = DateTime.ParseExact(data[5], "dd.MM.yyyy", null),
                        BirthPlace = data[6]
                    };
                    count++;
                }
            }

            Array.Resize(ref workers, count);
            return workers;
        }

        return null;
    }
}

internal class Program
{
    private static readonly Repository repository = new Repository("employees.txt");

    static void Main()
    {
        Console.WriteLine($"Текущая директория: {Environment.CurrentDirectory}");

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Вывести данные на экран");
            Console.WriteLine("2 - Заполнить данные и добавить новую запись");
            Console.WriteLine("3 - Просмотреть все записи");
            Console.WriteLine("4 - Удалить запись по ID");
            Console.WriteLine("5 - Загрузить записи в выбранном диапазоне дат");
            Console.WriteLine("0 - Выход");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Выберите поле для сортировки:");
                        Console.WriteLine("1 - По ID");
                        Console.WriteLine("2 - По дате добавления");
                        Console.WriteLine("3 - По Ф.И.О.");

                        int sortField;
                        if (int.TryParse(Console.ReadLine(), out sortField))
                        {
                            DisplayData(sortField);
                        }
                        else
                        {
                            Console.WriteLine("Некорректный выбор");
                        }
                        break;
                    case 2:
                        AddEmployee();
                        break;
                    case 3:
                        DisplayAllWorkers();
                        break;
                    case 4:
                        DeleteEmployee();
                        break;
                    case 5:
                        LoadWorkersByDateRange();
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод");
            }
        }
    }

    static void DisplayData(int sortField)
    {
        Worker[] workers = repository.GetAllWorkers();

        if (workers != null)
        {
            switch (sortField)
            {
                case 1:
                    Array.Sort(workers, (x, y) => x.Id.CompareTo(y.Id));
                    break;
                case 2:
                    Array.Sort(workers, (x, y) => x.AddedDateTime.CompareTo(y.AddedDateTime));
                    break;
                case 3:
                    Array.Sort(workers, (x, y) => x.FIO.CompareTo(y.FIO));
                    break;
                default:
                    Console.WriteLine("Некорректный выбор");
                    return;
            }

            foreach (var worker in workers)
            {
                Console.WriteLine($"ID: {worker.Id}, Дата и время добавления: {worker.AddedDateTime}, Ф. И. О.: {worker.FIO}, Возраст: {worker.Age}, Рост: {worker.Height}, Дата рождения: {worker.BirthDate}, Место рождения: {worker.BirthPlace}");
            }
        }
        else
        {
            Console.WriteLine("Файл не существует");
        }
    }

    static void AddEmployee()
    {
        Console.WriteLine("Введите данные нового сотрудника:");

        Console.Write("Ф. И. О.: ");
        string fullName = Console.ReadLine();

        Console.Write("Возраст: ");
        int age;
        if (!int.TryParse(Console.ReadLine(), out age))
        {
            Console.WriteLine("Некорректный возраст");
            return;
        }

        Console.Write("Рост: ");
        int height;
        if (!int.TryParse(Console.ReadLine(), out height))
        {
            Console.WriteLine("Некорректный рост");
            return;
        }

        Console.Write("Дата рождения (в формате ДД.ММ.ГГГГ): ");
        string birthDate = Console.ReadLine();

        Console.Write("Место рождения: ");
        string birthPlace = Console.ReadLine();

        Worker newWorker = new Worker
        {
            FIO = fullName,
            Age = age,
            Height = height,
            BirthDate = DateTime.ParseExact(birthDate, "dd.MM.yyyy", null),
            BirthPlace = birthPlace
        };

        repository.AddWorker(newWorker);
    }

    static void DisplayAllWorkers()
    {
        Worker[] workers = repository.GetAllWorkers();

        if (workers != null)
        {
            foreach (var worker in workers)
            {
                Console.WriteLine($"ID: {worker.Id}, Дата и время добавления: {worker.AddedDateTime}, Ф. И. О.: {worker.FIO}, Возраст: {worker.Age}, Рост: {worker.Height}, Дата рождения: {worker.BirthDate}, Место рождения: {worker.BirthPlace}");
            }
        }
        else
        {
            Console.WriteLine("Файл не существует");
        }
    }

    static void DeleteEmployee()
    {
        Console.Write("Введите ID записи для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            repository.DeleteWorker(id);
        }
        else
        {
            Console.WriteLine("Некорректный ID");
        }
    }

    static void LoadWorkersByDateRange()
    {
        Console.Write("Введите начальную дату (в формате ДД.ММ.ГГГГ): ");
        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateFrom))
        {
            Console.Write("Введите конечную дату (в формате ДД.ММ.ГГГГ): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTo))
            {
                Worker[] workers = repository.GetWorkersBetweenTwoDates(dateFrom, dateTo);

                if (workers != null)
                {
                    foreach (var worker in workers)
                    {
                        Console.WriteLine($"ID: {worker.Id}, Дата и время добавления: {worker.AddedDateTime}, Ф. И. О.: {worker.FIO}, Возраст: {worker.Age}, Рост: {worker.Height}, Дата рождения: {worker.BirthDate}, Место рождения: {worker.BirthPlace}");
                    }
                }
                else
                {
                    Console.WriteLine("Файл не существует");
                }
            }
            else
            {
                Console.WriteLine("Некорректная конечная дата");
            }
        }
        else
        {
            Console.WriteLine("Некорректная начальная дата");
        }
    }
}
