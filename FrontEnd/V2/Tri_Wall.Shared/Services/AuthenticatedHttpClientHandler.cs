using System.Net.Http.Headers;
using Tri_Wall.Shared.ViewModels;

namespace Tri_Wall.Shared.Services;

public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken.Token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}