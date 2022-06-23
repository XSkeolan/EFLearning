namespace Messenger.Options
{
    public class EmailOptions
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
    }
}
