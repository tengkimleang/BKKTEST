
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Common.Interfaces;

public interface IReportLayout
{
    Task<PrintViewLayoutResponse> CallViewLayout(string code, string docEntry, string path,string storeName);
}
