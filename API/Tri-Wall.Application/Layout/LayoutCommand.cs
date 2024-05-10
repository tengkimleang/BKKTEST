using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Layout;

public record LayoutCommand(
    string LayoutCode, 
    string DocEntry,
    string Path) : IRequest<ErrorOr<PrintViewLayoutResponse>>;
