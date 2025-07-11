using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using NetCorePal.Extensions.Repository;

namespace Quiz.Exam.Infrastructure.Repositories
{

    public interface IOrderRepository : IRepository<Order, OrderId>
    {

    }


    public class OrderRepository(ApplicationDbContext context) : RepositoryBase<Order, OrderId, ApplicationDbContext>(context), IOrderRepository
    {
    }
}
