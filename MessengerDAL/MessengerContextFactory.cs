using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDAL
{
    public class MessengerContextFactory : IDesignTimeDbContextFactory<MessengerContext>
    {
        public MessengerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();

            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("MessengerDb");

            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            var options = optionsBuilder.UseNpgsql(connectionString).Options;

            return new MessengerContext(options);
        }
    }
}
