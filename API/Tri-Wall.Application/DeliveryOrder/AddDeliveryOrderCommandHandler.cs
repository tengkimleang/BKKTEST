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
        Company oCompany = connection.Connect();
        unitOfWork.BeginTransaction();
        Documents oDeliveryOrder;
        oDeliveryOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
        oDeliveryOrder.CardCode = request.CardCode;
        oDeliveryOrder.ContactPersonCode = request.ContactPersonCode;
        oDeliveryOrder.NumAtCard = request.NumAtCard;
        oDeliveryOrder.Series = request.Series;
        oDeliveryOrder.DocDate = request.DocDate;
        oDeliveryOrder.DocDueDate = request.TaxDate;
        oDeliveryOrder.Comments = request.Remarks;
        oDeliveryOrder.BPL_IDAssignedToInvoice = request.BranchID;
        oDeliveryOrder.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oDeliveryOrder.Lines.ItemCode = l.ItemCode;
            oDeliveryOrder.Lines.Quantity = l.Qty;
            oDeliveryOrder.Lines.UnitPrice = l.Price;
            oDeliveryOrder.Lines.VatGroup = l.VatCode;
            oDeliveryOrder.Lines.WarehouseCode = l.WarehouseCode;
            if (l.BaseDocEntry != 0)
            {
                oDeliveryOrder.Lines.BaseEntry = Convert.ToInt32(l.BaseDocEntry);
                oDeliveryOrder.Lines.BaseType = 17;
                oDeliveryOrder.Lines.BaseLine = l.BaseLineNumber;
            }

            if (l.ManageItem == "S")
            {
                foreach (var serial in l.Serials)
                {
                    oDeliveryOrder.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SysNumber);
                    oDeliveryOrder.Lines.SerialNumbers.Add();
                }
            }
            else if (l.ManageItem == "B")
            {
                foreach (var batch in l.Batches)
                {
                    oDeliveryOrder.Lines.BatchNumbers.BatchNumber = batch.BatchOrSerialCode;
                    oDeliveryOrder.Lines.BatchNumbers.Quantity = batch.Qty;
                    oDeliveryOrder.Lines.BatchNumbers.Add();
                }
            }

            oDeliveryOrder.Lines.Add();
        }
        if (oDeliveryOrder.Add() == 0)
        {
            unitOfWork.Commit();
            return await Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
        }
        unitOfWork.Rollback();
        return await Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(), oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());

    }
}
