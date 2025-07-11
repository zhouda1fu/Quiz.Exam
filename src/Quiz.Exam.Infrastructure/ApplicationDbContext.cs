using MediatR;
using Microsoft.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

namespace Quiz.Exam.Infrastructure
{
    public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : AppDbContextBase(options, mediator)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            ConfigureStronglyTypedIdValueConverter(configurationBuilder);
            base.ConfigureConventions(configurationBuilder);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    }
}