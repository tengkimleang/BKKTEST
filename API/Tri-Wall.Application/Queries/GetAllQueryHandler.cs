using ErrorOr;
using MediatR;
using System;

using System.Data;
using Tri_Wall.Application.Common.Interfaces;
using Tri_Wall.Domain.DataProviders;

namespace Tri_Wall.Application.Queries;

public record GetAllQuery(
        DataProvider DataProvider) : IRequest<ErrorOr<DataTable>>;
public class GetAllQueryHandler : IRequestHandler<GetAllQuery, ErrorOr<DataTable>>
{
    IDataProviderRepository _dataProviderRepository;

    public GetAllQueryHandler(IDataProviderRepository dataProviderRepository)
    {
        _dataProviderRepository = dataProviderRepository;
    }

    public async Task<ErrorOr<DataTable>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        return await _dataProviderRepository.Query(request.DataProvider);
    }
}
