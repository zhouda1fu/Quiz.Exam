using Quiz.Exam.Domain.AggregatesModel.DeliverAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quiz.Exam.Infrastructure.EntityConfigurations
{
    internal class DeliverRecordConfiguration : IEntityTypeConfiguration<DeliverRecord>
    {
        public void Configure(EntityTypeBuilder<DeliverRecord> builder)
        {
            builder.ToTable("deliverrecord");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseSnowFlakeValueGenerator();
        }
    }

}
