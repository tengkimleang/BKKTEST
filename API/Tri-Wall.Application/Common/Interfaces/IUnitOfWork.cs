

namespace Tri_Wall.Application.Common.Interfaces;

public interface IUnitOfWork
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
