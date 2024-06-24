using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.GoodReceiptPo;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.IssueForProductions;

public class AddIssueForProductionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddIssueForProductionCommand, ErrorOr<PostResponse>>
{

    public Task<ErrorOr<PostResponse>> Handle(AddIssueForProductionCommand request, CancellationToken cancellationToken)
    {
        var oCompany = unitOfWork.Connect();
        oCompany.ThrowIfNull("Company is null");
        unitOfWork.BeginTransaction(oCompany);
        var oIssueForProduction = (Documents)oCompany.GetBusinessObject((request.IsDraft) ? BoObjectTypes.oDrafts : BoObjectTypes.oInventoryGenExit);
        if (!request.IsDraft) oIssueForProduction.DocObjectCode = BoObjectTypes.oInventoryGenExit;
        oIssueForProduction.Series = request.Series;
        oIssueForProduction.Reference2 = request.Remarks;
        oIssueForProduction.DocDate = request.DocDate;
        oIssueForProduction.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        foreach (var l in request.Lines!)
        {
            oIssueForProduction.Lines.Quantity = l.Qty;
            oIssueForProduction.Lines.BaseEntry = l.DocNum;
            oIssueForProduction.Lines.BaseLine = l.BaseLineNum;
            oIssueForProduction.Lines.BaseType = 202;
            oIssueForProduction.Lines.WarehouseCode = l.WhsCode;
            
            switch (l)
            {
                case {ManageItem:"S",Serials:not null}:
                {
                    foreach (var serial in l.Serials)
                    {
                        oIssueForProduction.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                        oIssueForProduction.Lines.SerialNumbers.Quantity = serial.Qty;
                        oIssueForProduction.Lines.SerialNumbers.ManufacturerSerialNumber =serial.MfrNo;
                        oIssueForProduction.Lines.SerialNumbers.ManufactureDate = Convert.ToDateTime(serial.MfrDate);
                        oIssueForProduction.Lines.SerialNumbers.ExpiryDate = Convert.ToDateTime(serial.ExpDate);
                        oIssueForProduction.Lines.SerialNumbers.Add();
                    }
                    break;
                }
                case { ManageItem: "B", Batches: not null }:
                {
                    foreach (var batch in l.Batches)
                    {
                        oIssueForProduction.Lines.BatchNumbers.AddmisionDate = DateTime.Now;
                        oIssueForProduction.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                        oIssueForProduction.Lines.BatchNumbers.Quantity = batch.Qty;
                        oIssueForProduction.Lines.BatchNumbers.ExpiryDate = (DateTime)batch.ExpDate!;
                        oIssueForProduction.Lines.BatchNumbers.ManufacturingDate = Convert.ToDateTime(batch.ManfectureDate);
                        oIssueForProduction.Lines.BatchNumbers.InternalSerialNumber = batch.LotNo;
                        oIssueForProduction.Lines.BatchNumbers.Add();
                    }
                    break;
                }
            }

            oIssueForProduction.Lines.Add();
        }
        if (oIssueForProduction.Add() != 0)
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