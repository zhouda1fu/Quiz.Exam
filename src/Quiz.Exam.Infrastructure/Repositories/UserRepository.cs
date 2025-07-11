using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Quiz.Exam.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User, UserId>;

public class UserRepository(ApplicationDbContext context) : RepositoryBase<User, UserId, ApplicationDbContext>(context), IUserRepository
{
   
} 