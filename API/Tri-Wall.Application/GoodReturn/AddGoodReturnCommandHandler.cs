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
        var oPurchaseReturn = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseReturns);
        oPurchaseReturn.CardCode = request.CustomerCode;
        oPurchaseReturn.ContactPersonCode = request.ContactPersonCode;
        oPurchaseReturn.NumAtCard = request.NumAtCard;
        oPurchaseReturn.Series = request.Series;
        oPurchaseReturn.DocDate = request.DocDate;
        oPurchaseReturn.DocDueDate = request.TaxDate;
        oPurchaseReturn.Comments = request.Remarks;
        oPurchaseReturn.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oPurchaseReturn.Lines.ItemCode = l.ItemCode;
            oPurchaseReturn.Lines.Quantity = l.Qty;
            oPurchaseReturn.Lines.UnitPrice = l.Price;
            oPurchaseReturn.Lines.VatGroup = l.VatCode;
            oPurchaseReturn.Lines.WarehouseCode = l.WarehouseCode;
            if (l.BaseEntry != 0)
            {
                oPurchaseReturn.Lines.BaseEntry = Convert.ToInt32(l.BaseEntry);
                oPurchaseReturn.Lines.BaseType = 20;
                oPurchaseReturn.Lines.BaseLine = l.BaseLine;
            }

            if (l.ManageItem == "S")
            {
                foreach (var serial in l.Serials!)
                {
                    //oPurchaseReturn.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SerialCode);
                    oPurchaseReturn.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                    oPurchaseReturn.Lines.SerialNumbers.Add();
                }
            }
            else if (l.ManageItem == "B")
            {
                foreach (var batch in l.Batches!)
                {
                    oPurchaseReturn.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                    oPurchaseReturn.Lines.BatchNumbers.Quantity = batch.Qty;
                    oPurchaseReturn.Lines.BatchNumbers.Add();
                }
            }

            oPurchaseReturn.Lines.Add();
        }

        if (oPurchaseReturn.Add() != 0)
        {
            unitOfWork.Rollback(oCompany);
            return Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(),
                oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());
        }

        unitOfWork.Commit(oCompany);
        return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
    }
}