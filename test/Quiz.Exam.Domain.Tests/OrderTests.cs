using Quiz.Exam.Domain.AggregatesModel.OrderAggregate;
namespace Quiz.Exam.Domain.Tests
{
    public class OrderTests
    {
        [Fact]
        public void OrderPaid_Test()
        {
            Order order = new("test", 1);
            Assert.False(order.Paid);
            order.OrderPaid();
            Assert.True(order.Paid);
        }
    }
}