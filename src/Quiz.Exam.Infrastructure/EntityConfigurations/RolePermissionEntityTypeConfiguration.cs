using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations;

internal class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission");
        
        builder.HasKey(t => new { t.RoleId, t.PermissionCode });
        
        builder.Property(b => b.RoleId);
        builder.Property(b => b.PermissionCode).HasMaxLength(100).IsRequired();
        builder.Property(b => b.PermissionName).HasMaxLength(100);
        builder.Property(b => b.PermissionDescription).HasMaxLength(200);
        
        // 外键关系
        builder.HasOne<Role>()
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 