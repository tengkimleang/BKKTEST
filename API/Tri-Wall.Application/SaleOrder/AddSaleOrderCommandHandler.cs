using ErrorOr;
using MediatR;
using SAPbobsCOM;
using Tri_Wall.Application.Common.Interfaces;
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

        public async Task<ErrorOr<PostResponse>> Handle(AddSaleOrderCommand request, CancellationToken cancellationToken)
        {
            Company oCompany = connection.Connect();
            unitOfWork.BeginTransaction();
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
            if (oSaleOrder.Add() == 0)
            {
                unitOfWork.Commit();
                return await Task.FromResult(new PostResponse("", "", "", "", oCompany.GetNewObjectKey()).ToErrorOr()).ConfigureAwait(false);
            }
            unitOfWork.Rollback();
            return await Task.FromResult(new PostResponse(oCompany.GetLastErrorCode().ToString(), oCompany.GetLastErrorDescription(), "", "", "").ToErrorOr()).ConfigureAwait(false);
        }
    }
}
