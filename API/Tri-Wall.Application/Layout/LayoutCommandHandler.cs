using ErrorOr;
using MediatR;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Layout;

public class LayoutCommandHandler(IReportLayout reportLayout)
    : IRequestHandler<LayoutCommand, PrintViewLayoutResponse>
{
    public async Task<PrintViewLayoutResponse> Handle(LayoutCommand request, CancellationToken cancellationToken)
    {
        if (request is { LayoutCode: not null, DocEntry: not null, Path: not null, StoreName: not null })
        {
            var result = await reportLayout.CallViewLayout(request.LayoutCode, request.DocEntry, request.Path, request.StoreName);
            return new PrintViewLayoutResponse(
                ErrCode: result.ErrCode,
                ErrorMessage: result.ErrorMessage,
                FileName: result.FileName,
                ApplicationType: result.ApplicationType,
                Data: result.Data);
        }

        return new PrintViewLayoutResponse(ErrCode: "1111", ErrorMessage: "Null");
    }
}
