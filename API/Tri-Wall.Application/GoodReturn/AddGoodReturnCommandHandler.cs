using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.GoodReturn;

public class AddGoodReturnCommandHandler : IRequestHandler<AddGoodReturnCommand, ErrorOr<PostResponse>>
{
    private readonly IUnitOfWork unitOfWork;

    public AddGoodReturnCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task<ErrorOr<PostResponse>> Handle(AddGoodReturnCommand request, CancellationToken cancellationToken)
    {
        Company oCompany = unitOfWork.Connect();
        unitOfWork.BeginTransaction(oCompany);
        var oDeliveryOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseReturns);
        oDeliveryOrder.CardCode = request.CustomerCode;
        oDeliveryOrder.ContactPersonCode = request.ContactPersonCode;
        oDeliveryOrder.NumAtCard = request.NumAtCard;
        oDeliveryOrder.Series = request.Series;
        oDeliveryOrder.DocDate = request.DocDate;
        oDeliveryOrder.DocDueDate = request.TaxDate;
        oDeliveryOrder.Comments = request.Remarks;
        oDeliveryOrder.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oDeliveryOrder.Lines.ItemCode = l.ItemCode;
            oDeliveryOrder.Lines.Quantity = l.Qty;
            oDeliveryOrder.Lines.UnitPrice = l.Price;
            oDeliveryOrder.Lines.VatGroup = l.VatCode;
            oDeliveryOrder.Lines.WarehouseCode = l.WarehouseCode;
            if (l.BaseEntry != 0)
            {
                oDeliveryOrder.Lines.BaseEntry = Convert.ToInt32(l.BaseEntry);
                oDeliveryOrder.Lines.BaseType = 20;
                oDeliveryOrder.Lines.BaseLine = l.BaseLine;
            }

            if (l.ManageItem == "S")
            {
                foreach (var serial in l.Serials!)
                {
                    //oDeliveryOrder.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SerialCode);
                    oDeliveryOrder.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                    oDeliveryOrder.Lines.SerialNumbers.Add();
                }
            }
            else if (l.ManageItem == "B")
            {
                foreach (var batch in l.Batches!)
                {
                    oDeliveryOrder.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                    oDeliveryOrder.Lines.BatchNumbers.Quantity = batch.Qty;
                    oDeliveryOrder.Lines.BatchNumbers.Add();
                }
            }

            oDeliveryOrder.Lines.Add();
        }

        if (oDeliveryOrder.Add() != 0)
        {
            unitOfWork.Rollback(oCompany);
            return Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(),
                oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());
        }

        unitOfWork.Commit(oCompany);
        return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
    }
}