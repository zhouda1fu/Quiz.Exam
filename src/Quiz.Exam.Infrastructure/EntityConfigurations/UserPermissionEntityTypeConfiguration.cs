using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations;

internal class UserPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermission");
        
        builder.HasKey(t => new { t.UserId, t.PermissionCode });
        
        builder.Property(b => b.UserId);
        builder.Property(b => b.PermissionCode).HasMaxLength(100).IsRequired();
        
        // 外键关系
        builder.HasOne<User>()
            .WithMany(u => u.Permissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 