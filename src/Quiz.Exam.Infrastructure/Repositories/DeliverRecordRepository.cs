using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using NetCorePal.Extensions.Repository;
using Quiz.Exam.Domain.AggregatesModel.DeliverAggregate;

namespace Quiz.Exam.Infrastructure.Repositories
{
    public interface IDeliverRecordRepository : IRepository<DeliverRecord, DeliverRecordId>
    {

    }

    public class DeliverRecordRepository(ApplicationDbContext context) : RepositoryBase<DeliverRecord, DeliverRecordId, ApplicationDbContext>(context), IDeliverRecordRepository
    {
    }
}
