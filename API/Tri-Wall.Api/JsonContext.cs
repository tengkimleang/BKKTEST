using Microsoft.AspNetCore.Identity.Data;
using System.Data;
using System.Text.Json.Serialization;
using Tri_Wall.Application;
using Tri_Wall.Application.Authorize;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Application.Queries;
using Tri_Wall.Application.SaleOrder;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[JsonSerializable(typeof(GetAllQuery))]
[JsonSerializable(typeof(DataTable))]
[JsonSerializable(typeof(PostResponse))]
[JsonSerializable(typeof(AddDeliveryOrderCommand))]
[JsonSerializable(typeof(AddDeliveryOrderCommandValidator))]
[JsonSerializable(typeof(AddSaleOrderCommand))]
[JsonSerializable(typeof(AddSaleOrderCommandValidator))]
[JsonSerializable(typeof(AuthorizeCommand))]
[JsonSerializable(typeof(AuthorizeCommandValidator))]
internal partial class AppJsonSeriaizerContext : JsonSerializerContext
{

}
