using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EmailAssistant.Models;

namespace EmailAssist.Data
{
    public class EmailAssistContext : DbContext
    {
        public EmailAssistContext (DbContextOptions<EmailAssistContext> options)
            : base(options)
        {
        }

        public DbSet<EmailAssistant.Models.Session> Session { get; set; } = default!;
        public DbSet<EmailAssistant.Models.Email> Email { get; set; } = default!;
        public DbSet<EmailAssistant.Models.Day> Day { get; set; } = default!;
        public DbSet<EmailAssistant.Models.Sender> Sender { get; set; } = default!;
    }
}
