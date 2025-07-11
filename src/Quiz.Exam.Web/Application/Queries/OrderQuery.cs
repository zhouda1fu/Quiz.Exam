using Quiz.Exam.Domain;
using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using Quiz.Exam.Infrastructure;
using System.Threading;

namespace Quiz.Exam.Web.Application.Queries
{
    public class OrderQuery(ApplicationDbContext applicationDbContext)
    {
        public async Task<Order?> QueryOrder(OrderId orderId, CancellationToken cancellationToken)
        {
            return await applicationDbContext.Orders.FindAsync(new object[] { orderId }, cancellationToken);
        }
    }
}
