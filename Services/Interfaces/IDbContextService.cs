using Ankietyzator.Models;

namespace Ankietyzator.Services.Interfaces
{
    public interface IDbContextService
    {
        AnkietyzatorDbContext Context { set; }
    }
}