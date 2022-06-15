using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerDAL
{
    public class MessengerContext : DbContext
    {
        public MessengerContext() { }

        public MessengerContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<UserType> UserTypes { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Models.File> Files { get; set; } = null!;
        public virtual DbSet<ChatLink> ChatLinks { get; set; } = null!;
        public virtual DbSet<ConfirmationCode> ConfirmationCodes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasMany(m => m.Files)
                .WithMany(f => f.Messages)
                .UsingEntity<MessageFile>(
                j => j
                    .HasOne(pt => pt.File)
                    .WithMany(t => t.MessageFiles)
                    .HasForeignKey(pt => pt.FileId),
                j => j
                    .HasOne(pt => pt.Message)
                    .WithMany(t => t.MessageFiles)
                    .HasForeignKey(pt => pt.MessageId),
                j =>
                {
                    j.HasKey(k => k.Id);
                    j.ToTable("MessageFile");
                });
        }
    }
}