﻿using ErrorOr;
using MediatR;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.Layout;

public record LayoutCommand : IRequest<PrintViewLayoutResponse>
{
    public string? LayoutCode { get; init; }
    public string? DocEntry { get; init; }
    public string? StoreName { get; init; }
    public string? Path { get; set; }
}