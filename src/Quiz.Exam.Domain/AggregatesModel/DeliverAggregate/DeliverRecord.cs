using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
using NetCorePal.Extensions.Domain;

namespace Quiz.Exam.Domain.AggregatesModel.DeliverAggregate
{
    public partial record DeliverRecordId : IInt64StronglyTypedId;

    public class DeliverRecord : Entity<DeliverRecordId>, IAggregateRoot
    {
        protected DeliverRecord() { }


        public DeliverRecord(OrderId orderId)
        {
            this.OrderId = orderId;
        }

        public OrderId OrderId { get; private set; } = default!;
    }
}
