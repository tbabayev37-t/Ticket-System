using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSistemi.Models;

namespace TicketSistemi.Context
{
    public sealed class ApplicationDbContext:IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Ticket>(entity =>
            {
                entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
                entity.Property(t=>t.Description).HasMaxLength(4000).IsRequired();
                entity.HasOne(t => t.CreatByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatByUserId)
                .OnDelete(DeleteBehavior.NoAction);
            });
            modelbuilder.Entity<TicketMessage>(entity =>
            {
                entity.Property(m => m.Text).HasMaxLength(4000).IsRequired();

                entity.HasOne(m => m.Ticket)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey(m => m.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            });
        }

    }
}
