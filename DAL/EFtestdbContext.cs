using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL
{
    public partial class EFtestdbContext : DbContext
    {
        public EFtestdbContext()
        {
        }

        public EFtestdbContext(DbContextOptions<EFtestdbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!; // для отношения один-к-одному
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Manager> Managers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseNpgsql("User ID=postgres;Password=hksk2q;Host=localhost;Port=5432;Database=EFtestdb;Pooling=true;Connection Lifetime=0;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>(); Включение сущности в модель БД
            //modelBuilder.Ignore<Company>(); Исключение сущности из модели БД

            // В моделях также можно испозовать сопоставление с полями
            //modelBuilder.Entity<User>().Property("Name").HasField("name");
            //modelBuilder.Entity<User>().Property("surname"); // и сами поля в модели могут быть полями в таблице

            // Создание составных ключей
            //modelBuilder.Entity<User>().HasKey(u => new { u.Email, u.Nickname });

            // Альтернативные ключи - являются аналогом ограничения на уникальность (также могут быть составными)
            //modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);

            // Создание индексов
            //modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Email").HasFilter("[Email] IS NOT NULL");

            // Задание значений по-умолчанию
            //modelBuilder.Entity<User>().Property(u => u.IsConfirmed).HasDefaultValue(false);
            //modelBuilder.Entity<Session>().Property(s => s.DateStart).HasDefaultValueSql("NOW()");

            // Задание вычисляемых столбцов
            //modelBuilder.Entity<User>().Property(u => u.Nickname).HasComputedColumnSql("Name || ' ' Surname");

            // Установка ограничений 
            //modelBuilder.Entity<Session>().HasCheckConstraint("DateEnd", "DateEnd > DateStart");

            // Создание конфигурации для модели User, при большой нагрузке на контекст в этом методе
            //modelBuilder.ApplyConfiguration(new CustomUserConfiguration());

            // Настройка внешниего ключа
            //modelBuilder.Entity<Session>().HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId);
            // Альтернативный внешний ключ
            //modelBuilder.Entity<Session>().HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.Name).HasPrincipalKey(u => u.Name);
            // при использовании можно будет установить как и навигационное свойство так и свойство внешнего ключа

            // Настройка каскадного удаления
            //modelBuilder.Entity<Session>().HasOne(s => s.User).WithMany(u => u.Sessions).OnDelete(DeleteBehavior.SetNull); // в чем разница в значениях перечисления ?????

            // Настройка отношения один-к-одному
            modelBuilder.Entity<User>().HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);

            // Объединение таблиц в отношении один-к-одному
            modelBuilder.Entity<User>().HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(up => up.Id);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserProfile>().ToTable("Users");

            // Настройка связи многие-ко-многим для переопределения названия таблицы
            modelBuilder.Entity<Course>().HasMany(c => c.Students).WithMany(s => s.Courses).UsingEntity(j => j.ToTable("Enrollments"));

            // Настройка связи многие-ко-многим для дополнительных данных в промежуточной таблице
            modelBuilder
            .Entity<Course>()
            .HasMany(c => c.Students)
            .WithMany(s => s.Courses)
            .UsingEntity<Enrollment>(
               j => j
                .HasOne(pt => pt.Student)
                .WithMany(t => t.Enrollments)
                .HasForeignKey(pt => pt.StudentId), // связь с таблицей Students через StudentId
            j => j
                .HasOne(pt => pt.Course)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(pt => pt.CourseId), // связь с таблицей Courses через CourseId
            j =>
            {
                j.Property(pt => pt.Mark).HasDefaultValue(3);
                j.HasKey(t => new { t.CourseId, t.StudentId });
                j.ToTable("Enrollments");
            });
        }
    }
}
