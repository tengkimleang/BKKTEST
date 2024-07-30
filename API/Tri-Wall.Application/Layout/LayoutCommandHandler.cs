using ErrorOr;
using MediatR;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Layout;

public class LayoutCommandHandler(IReportLayout reportLayout)
    : IRequestHandler<LayoutCommand, ErrorOr<PrintViewLayoutResponse>>
{
    public async Task<ErrorOr<PrintViewLayoutResponse>> Handle(LayoutCommand request, CancellationToken cancellationToken)
    {
        var result = await reportLayout.CallViewLayout(request.LayoutCode,request.DocEntry,request.Path);
        (result.ErrCode != "").Throw(result.ErrorMessage);
        return await Task.FromResult(new PrintViewLayoutResponse(ErrCode: result.ErrCode, ErrorMessage: result.ErrorMessage).ToErrorOr()).ConfigureAwait(false);
    }
}
