using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace MessengerDAL
{
    public class MessengerContextFactory : IDesignTimeDbContextFactory<MessengerContext>
    {
        public MessengerContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MessengerContext> optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();

            ConfigurationBuilder builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            IConfigurationRoot config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("MessengerDb");

            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            DbContextOptions<MessengerContext> options = optionsBuilder.UseNpgsql(connectionString).Options;

            return new MessengerContext(options);
        }
    }
}
