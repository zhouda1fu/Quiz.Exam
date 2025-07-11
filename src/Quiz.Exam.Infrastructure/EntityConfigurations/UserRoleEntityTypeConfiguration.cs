using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations;

internal class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole");
        
        builder.HasKey(t => new { t.UserId, t.RoleId });
        
        builder.Property(b => b.UserId);
        builder.Property(b => b.RoleId);
        builder.Property(b => b.RoleName).HasMaxLength(50).IsRequired();
        
        // 外键关系
        builder.HasOne<User>()
            .WithMany(u => u.Roles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 