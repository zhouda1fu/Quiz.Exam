using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
            
            builder.Property(b => b.Name).HasMaxLength(50).IsRequired();
            builder.Property(b => b.Email).HasMaxLength(100).IsRequired();
            builder.Property(b => b.PasswordHash).HasMaxLength(255).IsRequired();
            builder.Property(b => b.IsActive);
            builder.Property(b => b.CreatedAt);
            builder.Property(b => b.LastLoginTime);
            builder.Property(b => b.UpdateTime);
            
            // 唯一索引
            builder.HasIndex(b => b.Name).IsUnique();
            builder.HasIndex(b => b.Email).IsUnique();

            builder.HasMany(au => au.Roles)
            .WithOne()
            .HasForeignKey(aur => aur.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
            builder.Navigation(au => au.Roles).AutoInclude();

            // 配置 AdminUser 与 AdminUserPermission 的一对多关系
            builder.HasMany(au => au.Permissions)
                .WithOne()
                .HasForeignKey(aup => aup.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Navigation(au => au.Permissions).AutoInclude();

            builder.HasQueryFilter(au => !au.IsDeleted);
        }
    }
} 