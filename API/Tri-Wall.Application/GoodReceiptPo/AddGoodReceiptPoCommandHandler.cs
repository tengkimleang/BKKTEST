using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.GoodReceiptPo;

public class AddGoodReceiptPoCommandHandler : IRequestHandler<AddGoodReceiptPoCommand, ErrorOr<PostResponse>>
{
    private readonly IUnitOfWork unitOfWork;
    public AddGoodReceiptPoCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public Task<ErrorOr<PostResponse>> Handle(AddGoodReceiptPoCommand request, CancellationToken cancellationToken)
    {
        Company oCompany = unitOfWork.Connect();
        oCompany.ThrowIfNull("Company is null");
        unitOfWork.BeginTransaction(oCompany);
        Documents oGoodReceiptPO;
        oGoodReceiptPO = (Documents)oCompany.GetBusinessObject((request.IsDraft) ? BoObjectTypes.oDrafts : BoObjectTypes.oPurchaseDeliveryNotes);
        if (!request.IsDraft) oGoodReceiptPO.DocObjectCode = BoObjectTypes.oPurchaseDeliveryNotes;
        oGoodReceiptPO.CardCode = request.VendorCode;
        oGoodReceiptPO.ContactPersonCode = request.ContactPersonCode;
        oGoodReceiptPO.Series = request.Series;
        oGoodReceiptPO.DocDate = request.DocDate;
        oGoodReceiptPO.DocDueDate = request.TaxDate;
        oGoodReceiptPO.Comments = request.Remarks;
        oGoodReceiptPO.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oGoodReceiptPO.Lines.ItemCode = l.ItemCode;
            oGoodReceiptPO.Lines.Quantity = l.Qty;
            oGoodReceiptPO.Lines.UnitPrice = l.Price;
            oGoodReceiptPO.Lines.VatGroup = l.VatCode;
            oGoodReceiptPO.Lines.WarehouseCode = l.WarehouseCode;

            if (l.ManageItem == "S")
                foreach (var serial in l.Serials)
                {
                    oGoodReceiptPO.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                    oGoodReceiptPO.Lines.SerialNumbers.Quantity = 1;
                    oGoodReceiptPO.Lines.SerialNumbers.ManufacturerSerialNumber = serial.MfrNo == null ? "" : serial.MfrNo;
                    oGoodReceiptPO.Lines.SerialNumbers.ManufactureDate = Convert.ToDateTime(serial.MfrDate);
                    oGoodReceiptPO.Lines.SerialNumbers.ExpiryDate = Convert.ToDateTime(serial.ExpDate);
                    oGoodReceiptPO.Lines.SerialNumbers.Add();
                }
            else if (l.ManageItem == "B")
                foreach (var batch in l.Batches)
                {
                    oGoodReceiptPO.Lines.BatchNumbers.AddmisionDate = DateTime.Now;
                    oGoodReceiptPO.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                    oGoodReceiptPO.Lines.BatchNumbers.Quantity = batch.Qty;
                    oGoodReceiptPO.Lines.BatchNumbers.ExpiryDate = batch.ExpDate;
                    oGoodReceiptPO.Lines.BatchNumbers.ManufacturingDate = Convert.ToDateTime(batch.ManfectureDate);
                    oGoodReceiptPO.Lines.BatchNumbers.InternalSerialNumber = batch.LotNo;
                    oGoodReceiptPO.Lines.BatchNumbers.Add();
                }

            oGoodReceiptPO.Lines.Add();
        }
        if (oGoodReceiptPO.Add() != 0)
        {
            unitOfWork.Rollback(oCompany);
            return Task.FromResult(new PostResponse(
                oCompany.GetLastErrorCode().ToString(),
                oCompany.GetLastErrorDescription(),
                "",
                "",
                "").ToErrorOr());
        }
        unitOfWork.Commit(oCompany);
        return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
    }
}
