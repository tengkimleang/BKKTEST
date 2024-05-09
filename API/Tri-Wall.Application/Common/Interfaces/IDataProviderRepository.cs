
using System.Data;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IDataProviderRepository
{
    Task<DataTable> Query(DataProvider dataProviderRequest);
}
