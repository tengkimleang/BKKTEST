
using ErrorOr;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Common.ErrorHandling;

public static class ErrorHandlingHelper
{
    public static async Task<ErrorOr<PostResponse>> ExecuteWithHandlingAsync(Func<Task<ErrorOr<PostResponse>>> action, IUnitOfWork unitOfWork, Company oCompany)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback(oCompany);
            return new PostResponse(
                "Exception",
                ex.Message,
                "",
                "",
                "").ToErrorOr();
        }
    }
}