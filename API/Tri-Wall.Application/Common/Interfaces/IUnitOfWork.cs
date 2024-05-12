

using SAPbobsCOM;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Company Connect();
    void BeginTransaction(Company company);
    void Commit(Company company);
    void Rollback(Company company);
}
