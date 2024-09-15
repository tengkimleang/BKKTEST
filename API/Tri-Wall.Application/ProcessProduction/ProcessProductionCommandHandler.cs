using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.ErrorHandling;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.IssueForProductions;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.ProcessProduction;

public class ProcessProductionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ProcessProductionCommand, ErrorOr<PostResponse>>
{
    public Task<ErrorOr<PostResponse>> Handle(ProcessProductionCommand request, CancellationToken cancellationToken)
    {
        var oCompany = unitOfWork.Connect();
        return ErrorHandlingHelper.ExecuteWithHandlingAsync(() =>
        {
            oCompany.ThrowIfNull("Company is null");
            unitOfWork.BeginTransaction(oCompany);
            foreach (var obj in request.Data)
            {
                var oProductionOrders = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oProductionOrders);
                if (oProductionOrders.GetByKey(obj.ProductionNo))
                {
                    oProductionOrders.UserFields.Fields.Item("U_Status").Value = Guid.NewGuid().ToString();
                    oProductionOrders.UserFields.Fields.Item("U_ProcessStage").Value = Guid.NewGuid().ToString();
                    if (oProductionOrders.Update() != 0)
                    {
                        unitOfWork.Rollback(oCompany);
                        return Task.FromResult(new PostResponse(
                            oCompany.GetLastErrorCode().ToString(),
                            oCompany.GetLastErrorDescription(),
                            "",
                            "",
                            "").ToErrorOr());
                    }
                }
            }

            unitOfWork.Commit(oCompany);
            return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
        }, unitOfWork, oCompany);
    }
}