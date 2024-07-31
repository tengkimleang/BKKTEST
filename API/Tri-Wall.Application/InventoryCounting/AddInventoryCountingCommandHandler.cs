﻿using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.InventoryCounting;

public class AddInventoryCountingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddInventoryCountingCommand, ErrorOr<PostResponse>>
{
    public Task<ErrorOr<PostResponse>> Handle(AddInventoryCountingCommand request, CancellationToken cancellationToken)
    {
        var oCompany = unitOfWork.Connect();
        oCompany.ThrowIfNull("Company is null");
        unitOfWork.BeginTransaction(oCompany);
        var oCompanyService = oCompany.GetCompanyService();
        var oInventoryCountingService =
            (InventoryCountingsService)oCompanyService.GetBusinessService(ServiceTypes.InventoryCountingsService);
        var oInventoryCountingParams =
            (InventoryCountingParams)oInventoryCountingService.GetDataInterface(
                InventoryCountingsServiceDataInterfaces.icsInventoryCountingParams);
        oInventoryCountingParams.DocumentEntry = request.DocEntry;
        var oInventoryCounting = oInventoryCountingService.Get(oInventoryCountingParams);
        oInventoryCounting.Reference2 = request.Ref2;
        oInventoryCounting.Remarks = request.OtherRemark;
        oInventoryCounting.UserFields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
        var oInventoryCountingLines = oInventoryCounting.InventoryCountingLines;
        foreach (var counter in request.Counters)
        {
            for (var i = 0; i < oInventoryCountingLines.Count; i++) //Each Counter
            {
                foreach (var line in request.Lines) //Each Line
                {
                    var oInventoryCountingLine = oInventoryCountingLines.Item(i);
                    if ((oInventoryCountingLine.CounterID == counter.CountId &&
                         oInventoryCountingLine.ItemCode == line.ItemCode) || (oInventoryCountingLine.CounterID == -1 &&
                                                                               oInventoryCountingLine.ItemCode ==
                                                                               line.ItemCode))
                    {
                        oInventoryCountingLine.CountedQuantity = line.QtyCounted;
                        oInventoryCountingLine.UoMCode = line.UoM;
                        if (line.ManageItem.Contains("S") == true)
                        {
                            foreach (var serial in line.Serials)
                            {
                                if (serial.ItemCode == line.ItemCode && !string.IsNullOrEmpty(serial.SerialCode))
                                {
                                    if (serial.ConditionSerial == TypeSerial.NEW)
                                    {
                                        InventoryCountingSerialNumber oInventoryCountingSerialNumber =
                                            oInventoryCountingLine.InventoryCountingSerialNumbers.Add();
                                        oInventoryCountingSerialNumber.InternalSerialNumber = serial.SerialCode;
                                        oInventoryCountingSerialNumber.ManufacturerSerialNumber = serial.MfrNo;
                                        oInventoryCountingSerialNumber.ExpiryDate = Convert.ToDateTime(serial.ExpDate);
                                        oInventoryCountingSerialNumber.ManufactureDate =
                                            Convert.ToDateTime(serial.MfrDate);
                                        oInventoryCountingSerialNumber.Location = serial.Location;
                                        oInventoryCountingSerialNumber.ReceptionDate =
                                            Convert.ToDateTime(serial.ReceiptDate);
                                    }
                                    else if (serial.ConditionSerial == TypeSerial.OLD)
                                    {
                                        InventoryCountingSerialNumber oInventoryCountingSerialNumber =
                                            oInventoryCountingLine.InventoryCountingSerialNumbers.Add();
                                        oInventoryCountingSerialNumber.InternalSerialNumber = serial.SerialCode;
                                    }
                                }
                            }
                        }

                        else if (line.ManageItem.Contains("B") == true)
                        {
                            foreach (var batch in line.Batches)
                            {
                                if (batch.ItemCode == line.ItemCode && batch.BinEntry == line.BinEntry &&
                                    !string.IsNullOrEmpty(batch.BatchCode))
                                {
                                    if (batch.ConditionBatch == TypeSerial.NEW)
                                    {
                                        InventoryCountingBatchNumber oInventoryCountingBatchNumber =
                                            oInventoryCountingLine.InventoryCountingBatchNumbers.Add();
                                        oInventoryCountingBatchNumber.BatchNumber = batch.BatchCode;
                                        oInventoryCountingBatchNumber.InternalSerialNumber = batch.BatchCode;
                                        oInventoryCountingBatchNumber.Quantity = batch.Qty;
                                    }
                                    else if (batch.ConditionBatch == TypeSerial.OLD)
                                    {
                                        InventoryCountingBatchNumber oInventoryCountingBatchNumber =
                                            oInventoryCountingLine.InventoryCountingBatchNumbers.Add();
                                        oInventoryCountingBatchNumber.BatchNumber = batch.BatchCode;
                                        oInventoryCountingBatchNumber.InternalSerialNumber = batch.BatchCode;
                                        oInventoryCountingBatchNumber.Quantity = line.QtyCounted;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        oInventoryCountingService.Update(oInventoryCounting);
        if (oInventoryCounting.DocumentEntry != 0)
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