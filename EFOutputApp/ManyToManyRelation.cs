using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFOutputApp
{
    public class ManyToManyRelation
    {
        private readonly DbContextOptions<EFtestdbContext> _options;
        public ManyToManyRelation(DbContextOptions<EFtestdbContext> options)
        {
            _options = options;
        }

        public void AddData()
        {
            using (EFtestdbContext db = new EFtestdbContext(_options))
            {
                // создание и добавление моделей
                Student tom = new Student { Name = "Tom" };
                Student alice = new Student { Name = "Alice" };
                Student bob = new Student { Name = "Bob" };
                db.Students.AddRange(tom, alice, bob);

                Course algorithms = new Course { Name = "Алгоритмы" };
                Course basics = new Course { Name = "Основы программирования" };
                db.Courses.AddRange(algorithms, basics);

                // добавляем к студентам курсы
                tom.Courses.Add(algorithms);
                tom.Courses.Add(basics);
                alice.Courses.Add(algorithms);
                bob.Courses.Add(basics);

                // Можно было и наоборот - добавить студентов к курсу
                //algorithms.Students.AddRange(new List<Student>() { tom, bob });

                db.SaveChanges();
            }
        }

        public void GetData()
        {
            using (EFtestdbContext db = new EFtestdbContext())
            {
                var courses = db.Courses.Include(c => c.Students).ToList();
                // выводим все курсы
                foreach (var c in courses)
                {
                    Console.WriteLine($"Course: {c.Name}");
                    // выводим всех студентов для данного кура
                    foreach (Student s in c.Students)
                        Console.WriteLine($"Name: {s.Name}");
                    Console.WriteLine("-------------------");
                }
            }
        }

        public void UpdateData()
        {
            using (EFtestdbContext db = new EFtestdbContext(_options))
            {
                Student? alice = db.Students.Include(s => s.Courses).FirstOrDefault(s => s.Name == "Alice");
                Course? algorithms = db.Courses.FirstOrDefault(c => c.Name == "Алгоритмы");
                Course? basics = db.Courses.FirstOrDefault(c => c.Name == "Основы программирования");
                if (alice != null && algorithms != null && basics != null)
                {
                    // удаление курса у студента
                    alice.Courses.Remove(algorithms);
                    // добавление нового курса студенту
                    alice.Courses.Add(basics);
                    db.SaveChanges();
                }
            }
        }

        public void DeleteData()
        {
            using (EFtestdbContext db = new EFtestdbContext(_options))
            {
                // все строки в промежуточной таблице тоже будут удалены
                Student? student = db.Students.FirstOrDefault();
                db.Students.Remove(student);
                db.SaveChanges();
            }
        }
    }
}
