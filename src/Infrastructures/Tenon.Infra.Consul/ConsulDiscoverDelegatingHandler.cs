﻿using Microsoft.Extensions.Options;
using Tenon.Infra.Consul.Configurations;
using Tenon.Infra.Consul.LoadBalancer;

namespace Tenon.Infra.Consul;

public sealed class ConsulDiscoverDelegatingHandler(
    IOptions<ConsulDiscoveryOptions> consulDiscoveryOptions,
    ILoadBalancer loadBalancer)
    : DelegatingHandler
{
    private readonly ServiceDiscoveryProvider _serviceDiscoveryProvider = new(consulDiscoveryOptions, loadBalancer);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var currentUri = request.RequestUri;
        if (currentUri is null)
            throw new NullReferenceException(nameof(request.RequestUri));
        try
        {
            var baseUri = await _serviceDiscoveryProvider.GetSingleHealthServiceAsync();
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new NullReferenceException($"{currentUri.Host} does not contain helath service address!");
            var realRequestUri = new Uri($"{currentUri.Scheme}://{baseUri}{currentUri.PathAndQuery}");
            request.RequestUri = realRequestUri;
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            request.RequestUri = currentUri;
        }
    }
}