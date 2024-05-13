using Microsoft.Extensions.Http.Resilience;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Shared.Services;

public class WebOrMobileHttpRetryStrategyOptions : HttpRetryStrategyOptions
{
    public WebOrMobileHttpRetryStrategyOptions()
    {
        BackoffType = DelayBackoffType.Exponential;
        MaxRetryAttempts = 3;
        UseJitter = true;
        Delay = TimeSpan.FromSeconds(1.5);
    }
}
