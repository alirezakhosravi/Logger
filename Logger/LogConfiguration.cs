using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Raveshmand.Logger
{
    public class LogConfiguration : IEntityTypeConfiguration<LogRecord>
    {
        public void Configure(EntityTypeBuilder<LogRecord> builder)
        {
            builder.ToTable(nameof(LogRecord), "dbo");

            builder.HasKey(a => a.Id);

            builder.HasIndex(e => e.Logger).HasName("IX_Log_Logger");
            builder.HasIndex(e => e.Level).HasName("IX_Log_Level");

            builder.Property(a => a.Level).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Message).IsRequired();
            builder.Property(a => a.Logger).HasMaxLength(256).IsRequired();
            builder.Property(a => a.Url).HasMaxLength(1024);
        }
    }
}
