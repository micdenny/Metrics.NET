using Metrics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Owin.Metrics.Middleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class HttpStatusCodeMeterMiddleware : MetricMiddleware
    {
        private readonly Meter httpStatusCodeMeter;
        private AppFunc next;

        public HttpStatusCodeMeterMiddleware(MetricsContext context, string metricName, Regex[] ignorePatterns)
            : base(ignorePatterns)
        {
            this.httpStatusCodeMeter = context.Meter(metricName, Unit.Errors);
        }

        public void Initialize(AppFunc next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (PerformMetric(environment))
            {
                await this.next(environment);

                this.httpStatusCodeMeter.Mark(environment["owin.ResponseStatusCode"].ToString());
            }
            else
            {
                await this.next(environment);
            }
        }
    }
}
