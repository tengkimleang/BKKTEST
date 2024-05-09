

namespace Tri_Wall.Domain.Common;

public record PostResponse(
    string ErrorCode = "",
    string ErrorMsg = "",
    string DocNum = "",
    string EDocNum = "",
    string DocEntry = "");
