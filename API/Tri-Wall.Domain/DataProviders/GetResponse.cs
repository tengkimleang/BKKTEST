
using System.Data;

namespace Tri_Wall.Domain.DataProviders;

public record GetResponse(
    string ErrorCode = "",
    string ErrorMessage = "",
    DataTable Data = null!);
