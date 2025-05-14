using TreeAPI.Models;

namespace TreeAPI.Repositories.Interface;

public interface IExceptionLogRepository : IRepository<ExceptionLog>
{
    Task LogExceptionAsync(ExceptionLog log);
}
