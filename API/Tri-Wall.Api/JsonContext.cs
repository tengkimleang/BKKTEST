using Microsoft.AspNetCore.Identity.Data;
using System.Data;
using System.Text.Json.Serialization;
using Tri_Wall.Application.DeliveryOrder;
using Tri_Wall.Application.Queries;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.API;

[JsonSerializable(typeof(DataTable))]
[JsonSerializable(typeof(AddDeliveryOrderCommand))]
[JsonSerializable(typeof(PostResponse))]
[JsonSerializable(typeof(RegisterRequest))]
[JsonSerializable(typeof(GetAllQuery))]
internal partial class AppJsonSeriaizerContext : JsonSerializerContext
{

}
