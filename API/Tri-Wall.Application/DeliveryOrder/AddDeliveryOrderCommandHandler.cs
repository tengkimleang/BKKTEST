using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.DeliveryOrder;

public class AddDeliveryOrderCommandHandler : IRequestHandler<AddDeliveryOrderCommand, ErrorOr<PostResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IConnection connection;

    public AddDeliveryOrderCommandHandler(IUnitOfWork unitOfWork, IConnection connection)
    {
        this.unitOfWork = unitOfWork;
        this.connection = connection;
    }

    public async Task<ErrorOr<PostResponse>> Handle(AddDeliveryOrderCommand request, CancellationToken cancellationToken)
    {
        unitOfWork.BeginTransaction();
        Documents oGoodReceiptPO;
        Company oCompany;
        oCompany = connection.Connect();
        oGoodReceiptPO = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
        oGoodReceiptPO.CardCode = request.CardCode;
        oGoodReceiptPO.ContactPersonCode = request.ContactPersonCode;
        oGoodReceiptPO.NumAtCard = request.NumAtCard;
        oGoodReceiptPO.Series = request.Series;
        oGoodReceiptPO.DocDate = request.DocDate;
        oGoodReceiptPO.DocDueDate = request.TaxDate;
        oGoodReceiptPO.Comments = request.Remarks;
        oGoodReceiptPO.BPL_IDAssignedToInvoice = request.BranchID;
        oGoodReceiptPO.UserFields.Fields.Item("U_WebID").Value = Guid.NewGuid();
        foreach (var l in request.Lines!)
        {
            oGoodReceiptPO.Lines.ItemCode = l.ItemCode;
            oGoodReceiptPO.Lines.Quantity = l.Qty;
            oGoodReceiptPO.Lines.UnitPrice = l.Price;
            oGoodReceiptPO.Lines.VatGroup = l.VatCode;
            oGoodReceiptPO.Lines.WarehouseCode = l.WarehouseCode;
            if (l.BaseDocEntry != 0)
            {
                oGoodReceiptPO.Lines.BaseEntry = Convert.ToInt32(l.BaseDocEntry);
                oGoodReceiptPO.Lines.BaseType = 17;
                oGoodReceiptPO.Lines.BaseLine = l.BaseLineNumber;
            }

            if (l.ManageItem == "S")
            {
                foreach (var serial in l.Serials)
                {
                    oGoodReceiptPO.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SysNumber);
                    oGoodReceiptPO.Lines.SerialNumbers.Add();
                }
            }
            else if (l.ManageItem == "B")
            {
                foreach (var batch in l.Batches)
                {
                    oGoodReceiptPO.Lines.BatchNumbers.BatchNumber = batch.BatchOrSerialCode;
                    oGoodReceiptPO.Lines.BatchNumbers.Quantity = batch.Qty;
                    oGoodReceiptPO.Lines.BatchNumbers.Add();
                }
            }

            oGoodReceiptPO.Lines.Add();
        }
        if (oGoodReceiptPO.Add() == 0)
        {
            unitOfWork.Commit();
            return await Task.FromResult(new PostResponse(0, "", "", "", "").ToErrorOr());
        }
        unitOfWork.Rollback();
        return await Task.FromResult(new PostResponse(1, oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());
    }
}
