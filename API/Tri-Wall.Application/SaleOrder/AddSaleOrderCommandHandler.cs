using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Throw;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Application.Common.Interfaces.Setting;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.SaleOrder
{
    public class AddSaleOrderCommandHandler : IRequestHandler<AddSaleOrderCommand, ErrorOr<PostResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConnection connection;

        public AddSaleOrderCommandHandler(IUnitOfWork unitOfWork, IConnection connection)
        {
            this.unitOfWork = unitOfWork;
            this.connection = connection;
        }

        public Task<ErrorOr<PostResponse>> Handle(AddSaleOrderCommand request, CancellationToken cancellationToken)
        {
            Company oCompany = connection.Connect();
            oCompany.ThrowIfNull("Invalid Connection Company");
            unitOfWork.BeginTransaction(oCompany);
            Documents oSaleOrder;
            oSaleOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
            oSaleOrder.CardCode = request.CardCode;
            oSaleOrder.ContactPersonCode = request.ContactPersonCode;
            oSaleOrder.NumAtCard = request.NumAtCard;
            oSaleOrder.Series = request.Series;
            oSaleOrder.DocDate = request.DocDate;
            oSaleOrder.DocDueDate = request.TaxDate;
            oSaleOrder.Comments = request.Remarks;
            oSaleOrder.BPL_IDAssignedToInvoice = request.BranchID;
            oSaleOrder.UserFields.Fields.Item("U_WEBID").Value = Guid.NewGuid().ToString();
            foreach (var l in request.Lines!)
            {
                oSaleOrder.Lines.ItemCode = l.ItemCode;
                oSaleOrder.Lines.Quantity = l.Qty;
                oSaleOrder.Lines.UnitPrice = l.Price;
                oSaleOrder.Lines.VatGroup = l.VatCode;
                oSaleOrder.Lines.WarehouseCode = l.WarehouseCode;
                oSaleOrder.Lines.Add();
            }
            (oSaleOrder.Add() != 0).Throw(oCompany.GetLastErrorDescription());
            unitOfWork.Commit(oCompany);
            return Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr());
        }
    }
}
