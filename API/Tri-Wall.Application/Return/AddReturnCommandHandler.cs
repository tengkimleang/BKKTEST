﻿using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.ErrorHandling;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Return;

public class AddReturnCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddReturnCommand, ErrorOr<PostResponse>>
{
    public Task<ErrorOr<PostResponse>> Handle(AddReturnCommand request, CancellationToken cancellationToken)
    {
        var oCompany = unitOfWork.Connect();
        return ErrorHandlingHelper.ExecuteWithHandlingAsync(() =>
        {
            unitOfWork.BeginTransaction(oCompany);
            var oReturns = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oReturns);
            oReturns.CardCode = request.CustomerCode;
            oReturns.ContactPersonCode = request.ContactPersonCode;
            oReturns.NumAtCard = request.NumAtCard;
            oReturns.Series = request.Series;
            oReturns.DocDate = request.DocDate;
            oReturns.DocDueDate = request.TaxDate;
            oReturns.Comments = request.Remarks;
            oReturns.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
            foreach (var l in request.Lines!)
            {
                oReturns.Lines.ItemCode = l.ItemCode;
                oReturns.Lines.Quantity = l.Qty;
                oReturns.Lines.UnitPrice = l.Price;
                oReturns.Lines.VatGroup = l.VatCode;
                oReturns.Lines.WarehouseCode = l.WarehouseCode;
                if (l.BaseEntry != 0)
                {
                    oReturns.Lines.BaseEntry = Convert.ToInt32(l.BaseEntry);
                    oReturns.Lines.BaseType = 15;
                    oReturns.Lines.BaseLine = l.BaseLine;
                }

                if (l.ManageItem == "S")
                {
                    foreach (var serial in l.Serials!)
                    {
                        //oReturns.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt32(serial.SerialCode);
                        oReturns.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                        oReturns.Lines.SerialNumbers.Add();
                    }
                }
                else if (l.ManageItem == "B")
                {
                    foreach (var batch in l.Batches!)
                    {
                        oReturns.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                        oReturns.Lines.BatchNumbers.Quantity = batch.Qty;
                        oReturns.Lines.BatchNumbers.Add();
                    }
                }

                oReturns.Lines.Add();
            }

            if (oReturns.Add() != 0)
            {
                unitOfWork.Rollback(oCompany);
                return Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(),
                    oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr());
            }

            unitOfWork.Commit(oCompany);
            return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
        }, unitOfWork, oCompany);
    }
}