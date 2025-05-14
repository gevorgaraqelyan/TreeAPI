using TreeAPI.Data;
using TreeAPI.Models;
using TreeAPI.Repositories.Interface;

namespace TreeAPI.Repositories;

public class ExceptionLogRepository : Repository<ExceptionLog>, IExceptionLogRepository
{
    public ExceptionLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task LogExceptionAsync(ExceptionLog log)
    {
        await AddAsync(log);
        await SaveChangesAsync();
    }
}