using MediatR;
using ErrorOr;
using Tri_Wall.Domain.Common;

namespace Tri_Wall.Application.ProcessProduction;

public record ProcessProductionCommand(
    List<ProcessProductionLine> Data
) : IRequest<ErrorOr<PostResponse>>;

public record ProcessProductionLine(
    int ProductionNo,
    string ProcessStage,
    string Status
);