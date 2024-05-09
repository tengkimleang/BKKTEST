

namespace Tri_Wall.Domain.Common;

public record PostResponse(
    int ErrorCode,
    string ErrorMsg,
    string DocNum,
    string EDocNum,
    string DocEntry);
