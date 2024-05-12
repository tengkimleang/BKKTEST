using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.GoodReturn
{
    public class AddGoodReturnCommandHandler : IRequestHandler<AddGoodReturnCommand, ErrorOr<PostResponse>>
    {
        private readonly IConnection connection;
        private readonly IUnitOfWork unitOfWork;
        public AddGoodReturnCommandHandler(IConnection connection, IUnitOfWork unitOfWork)
        {
            this.connection = connection;
            this.unitOfWork = unitOfWork;
        }

        public Task<ErrorOr<PostResponse>> Handle(AddGoodReturnCommand request, CancellationToken cancellationToken)
        {
            Documents oGoodReturn;
            Company oCompany;
            oCompany = connection.Connect();
            oCompany.Throw("Invalid Connection to SAP");
            unitOfWork.BeginTransaction(oCompany);
            oGoodReturn = (Documents)oCompany.GetBusinessObject((request.IsDraft) ? BoObjectTypes.oDrafts : BoObjectTypes.oPurchaseReturns);
            if (request.IsDraft) oGoodReturn.DocObjectCode = BoObjectTypes.oPurchaseReturns;
            oGoodReturn.CardCode = request.VendorCode;
            oGoodReturn.ContactPersonCode = request.ContactPersonCode;
            oGoodReturn.NumAtCard = request.NumAtCard;
            oGoodReturn.Series = request.Series;
            oGoodReturn.DocDate = request.DocDate;
            oGoodReturn.DocDueDate = request.TaxDate;
            oGoodReturn.Comments = request.Remarks;
            oGoodReturn.UserFields.Fields.Item("U_WebID").Value = Guid.NewGuid().ToString();
            foreach (var l in request.Lines)
            {
                oGoodReturn.Lines.ItemCode = l.ItemCode;
                oGoodReturn.Lines.Quantity = l.Qty;
                oGoodReturn.Lines.UnitPrice = l.Price;
                oGoodReturn.Lines.VatGroup = l.VatCode;
                oGoodReturn.Lines.WarehouseCode = l.WarehouseCode;
                if (l.BaseDocEntry != 0)
                {
                    oGoodReturn.Lines.BaseEntry = l.BaseDocEntry;
                    oGoodReturn.Lines.BaseType = 20;
                    oGoodReturn.Lines.BaseLine = l.BaseLineNumber;
                }

                if (l.ManageItem == "S")
                    foreach (var serial in l.Serials)
                    {
                        oGoodReturn.Lines.SerialNumbers.Quantity = 1;
                        oGoodReturn.Lines.SerialNumbers.InternalSerialNumber = serial.SerialCode;
                        oGoodReturn.Lines.SerialNumbers.Quantity = 1;
                        oGoodReturn.Lines.SerialNumbers.ManufacturerSerialNumber = serial.MfrNo;
                        oGoodReturn.Lines.SerialNumbers.ManufactureDate = serial.MfrDate;
                        oGoodReturn.Lines.SerialNumbers.ExpiryDate = serial.ExpDate;
                        oGoodReturn.Lines.SerialNumbers.Add();
                    }
                else if (l.ManageItem == "B")
                    foreach (var batch in l.Batches)
                    {
                        oGoodReturn.Lines.BatchNumbers.AddmisionDate = batch.AdmissionDate;
                        oGoodReturn.Lines.BatchNumbers.BatchNumber = batch.BatchCode;
                        oGoodReturn.Lines.BatchNumbers.Quantity = batch.Qty;
                        oGoodReturn.Lines.BatchNumbers.ExpiryDate = batch.ExpireDate;
                        oGoodReturn.Lines.BatchNumbers.AddmisionDate =
                        oGoodReturn.Lines.BatchNumbers.ManufacturingDate = batch.ManfectureDate;
                        oGoodReturn.Lines.BatchNumbers.InternalSerialNumber = batch.LotNo;
                        oGoodReturn.Lines.BatchNumbers.Add();
                    }

                oGoodReturn.Lines.Add();
            }
            (oGoodReturn.Add() != 0).Throw(oCompany.GetLastErrorDescription());
            unitOfWork.Commit(oCompany);
            return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
        }
    }
}
