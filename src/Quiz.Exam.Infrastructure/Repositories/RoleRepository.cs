using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using NetCorePal.Extensions.Repository;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;

namespace Quiz.Exam.Infrastructure.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId>;

public class RoleRepository : RepositoryBase<Role, RoleId, ApplicationDbContext>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

  
} 