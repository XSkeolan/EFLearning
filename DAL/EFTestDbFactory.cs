using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class EFTestDbFactory : IDesignTimeDbContextFactory<EFtestdbContext>
    {
        public EFtestdbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EFtestdbContext>();

            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла
            builder.AddJsonFile("appconfig.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("EFtestdb");

            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            var options = optionsBuilder.UseNpgsql(connectionString).Options;

            return new EFtestdbContext(options);
        }
    }
}
