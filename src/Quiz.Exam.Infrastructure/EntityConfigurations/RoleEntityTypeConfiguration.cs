using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations;

internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        
        builder.Property(b => b.Name).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Description).HasMaxLength(200);
        builder.Property(b => b.CreatedAt);
        builder.Property(b => b.IsActive);
        builder.Property(b => b.IsDeleted);
        builder.Property(b => b.DeletedTime);
        
        // 唯一索引
        builder.HasIndex(b => b.Name).IsUnique();

        builder.HasMany(r => r.Permissions).WithOne().HasForeignKey(rp => rp.RoleId);
        builder.Navigation(e => e.Permissions).AutoInclude();

        // 软删除过滤器
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
} 