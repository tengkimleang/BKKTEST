﻿
using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.ARCreditMemo;

public class AddARCreditMemoCommandHandler : IRequestHandler<AddDeliveryOrderCommand, ErrorOr<PostResponse>>
{
    private readonly IUnitOfWork unitOfWork;

    public AddARCreditMemoCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task<ErrorOr<PostResponse>> Handle(AddDeliveryOrderCommand request, CancellationToken cancellationToken)
    {
        Company oCompany = unitOfWork.Connect();
        unitOfWork.BeginTransaction(oCompany);
        var oCreditNotes = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);
        oCreditNotes.CardCode = request.CustomerCode;
        oCreditNotes.ContactPersonCode = request.ContactPersonCode;
        oCreditNotes.NumAtCard = request.NumAtCard;
        oCreditNotes.Series = request.Series;
        oCreditNotes.DocDate = request.DocDate;
        oCreditNotes.DocDueDate = request.TaxDate;
        oCreditNotes.Comments = request.Remarks;
        oCreditNotes.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oCreditNotes.Lines.ItemCode = l.ItemCode;
            oCreditNotes.Lines.Quantity = l.Qty;
            oCreditNotes.Lines.UnitPrice = l.Price;
            oCreditNotes.Lines.VatGroup = l.VatCode;
            oCreditNotes.Lines.WarehouseCode = l.WarehouseCode;
            if (l.BaseEntry != 0)
            {
                oCreditNotes.Lines.BaseEntry = Convert.ToInt32(l.BaseEntry);
                oCreditNotes.Lines.BaseType = 13;
                oCreditNotes.Lines.BaseLine = l.BaseLine;
            }

            if (l.ManageItem == "S")
            {
                foreach (var serial in l.Serials!)
                {
                    //oCreditNotes.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SerialCode);
                    oCreditNotes.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                    oCreditNotes.Lines.SerialNumbers.Add();
                }
            }
            else if (l.ManageItem == "B")
            {
                foreach (var batch in l.Batches!)
                {
                    oCreditNotes.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                    oCreditNotes.Lines.BatchNumbers.Quantity = batch.Qty;
                    oCreditNotes.Lines.BatchNumbers.Add();
                }
            }

            oCreditNotes.Lines.Add();
        }
        if (oCreditNotes.Add() != 0)
        {
            unitOfWork.Rollback(oCompany);
            return Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(), oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());
        }
        unitOfWork.Commit(oCompany);
        return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());

    }
}