using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IDataProviderRepository
{
    Task<DataTable> Query(DataProvider dataProviderRequest);
}
