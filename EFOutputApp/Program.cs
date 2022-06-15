using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
// установка пути к текущему каталогу
builder.SetBasePath(Directory.GetCurrentDirectory());
// получаем конфигурацию из файла
builder.AddJsonFile("appconfig.json");
// создаем конфигурацию
var config = builder.Build();
// получаем строку подключения
string connectionString = config.GetConnectionString("EFtestdb");

var optionsBuilder = new DbContextOptionsBuilder<EFtestdbContext>();
//optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
var options = optionsBuilder.UseNpgsql(connectionString).Options;

//CRUD-операции
//User? user1 = null;

//CREATE
//using(EFtestdbContext db = new EFtestdbContext(options))
//{
//    User tomUser = new User { Nickname = "Tom" };
//    User myUser = new User { Nickname = "skeo" };
//    db.Users.AddRange(tomUser, myUser);
//    db.SaveChanges();
//    Console.WriteLine("Users created");
//}

////READ
//using (EFtestdbContext db = new EFtestdbContext(options))
//{
//    user1 = db.Users.FirstOrDefault();
//    Console.WriteLine("First user - {0}", user1);
//}

////UPDATE
//using (EFtestdbContext db = new EFtestdbContext(options))
//{
//    //Если объект объявляется вне контекста данных, то EF его не отслеживает!!!
//    if (user1 != null)
//    {
//        Console.WriteLine("Данные до редактирования: {0}", user1);
//        user1.Nickname = "test";
//        db.Update(user1); // поэтому обновлять приходиться здесь 
//        db.SaveChanges();

//        user1 = db.Users.Find(user1.Id);
//        Console.WriteLine("Данные после редактирования: {0}", user1);
//    }

//    //Если же объект объявляется внутри контекста, то его состоянипе отслеживается
//    User? user2 = db.Users.Find(Guid.Parse("ef405076-c121-4ab6-82d3-1aadb50944e9"));
//    Console.WriteLine("Данные до редактирования объявленные в контексте: {0}", user2);
//    user2.IsDeleted = true;
//    db.SaveChanges();

//    user2 = db.Users.Find(Guid.Parse("ef405076-c121-4ab6-82d3-1aadb50944e9"));
//    Console.WriteLine("Данные после редактирования объявленные в контексте: {0}", user2);
//}

////DELETE
//using (EFtestdbContext db = new EFtestdbContext(options))
//{
//    user1 = db.Users.FirstOrDefault();
//    db.Users.Remove(user1);
//    db.SaveChanges();
//    Console.WriteLine("User Deleted {0}", user1);
//}

//// Вывод всех данных
//using (EFtestdbContext db = new EFtestdbContext(options))
//{
//    var users = db.Users.ToList();
//    foreach (var user in users)
//    {
//        Console.WriteLine(user);
//    }
//}

// Жадная загрузка данных
using (EFtestdbContext db = new EFtestdbContext(options))
{
    var sessions = db.Sessions.Include(s => s.User).ToList();
    foreach (var session in sessions)
    {
        Console.WriteLine($"{session.DateStart} - {session.User?.Nickname}");
    }

    var users = db.Users.Include(u => u.Sessions).ToList();
    foreach (var user in users)
    {
        Console.WriteLine(user.Nickname);
        foreach (var session in user.Sessions)
        {
            Console.WriteLine(session.DateStart);
        }
    }
}

// Явная загрузка данных
using (EFtestdbContext db = new EFtestdbContext(options))
{
    User? user = db.Users.FirstOrDefault();
    if(user != null)
    {
        db.Sessions.Where(s => s.UserId == user.Id).Load();
        Console.WriteLine($"User: {user.Nickname}");
        foreach (var s in user.Sessions)
        {
            Console.WriteLine($"Session: {s.DateStart} - {s.DateEnd}");
        }
    }
}

// Отложенная загрузка (useProxy)

// Использование подхода к наследованию TPH(Table Per Hierarchy)
using (EFtestdbContext db = new EFtestdbContext(options))
{
    User user1 = new User { Nickname = "Tom", Email="sdfsd@ree.ru", Password="password", Phone="89892423344" };
    User user2 = new User { Nickname = "Bob", Email = "ghgh@ree.ru", Password = "gggggggg", Phone = "89944923236" };
    db.Users.Add(user1);
    db.Users.Add(user2);

    Employee employee = new Employee { Nickname = "Sam", Salary = 500, Email = "kljkj@ree.ru", Password = "password", Phone = "8989240344" };
    db.Employees.Add(employee);

    Manager manager = new Manager { Nickname = "Robert", Departament = "IT", Email = "odmj@ree.ru", Password = "password", Phone = "80342423344" };
    db.Managers.Add(manager);

    db.SaveChanges();

    var users = db.Users.ToList();
    Console.WriteLine("Все пользователи");
    foreach (var user in users)
    {
        Console.WriteLine(user.Nickname);
    }

    Console.WriteLine("\n Все работники");
    foreach (var emp in db.Employees.ToList())
    {
        Console.WriteLine(emp.Nickname);
    }

    Console.WriteLine("\nВсе менеджеры");
    foreach (var man in db.Managers.ToList())
    {
        Console.WriteLine(man.Nickname);
    }
}

