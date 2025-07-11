using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using Quiz.Exam.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.DeliverAggregate;

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

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<DeliverRecord> DeliverRecords => Set<DeliverRecord>();
    }
}