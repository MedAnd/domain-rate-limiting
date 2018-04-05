using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Domain.RateLimiting.Core;

namespace Domain.RateLimiting.WebApi
{
    public class RateLimitingPostActionFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Request.Properties.ContainsKey("RateLimitingResult"))
                AddUpdateRateLimitingSuccessHeaders(actionExecutedContext,
                    (RateLimitingResult)actionExecutedContext.Request.Properties["RateLimitingResult"]);

            if (actionExecutedContext.Request.Properties.ContainsKey("PostActionFilterFuncAsync"))
            {
                var func = actionExecutedContext.Request.Properties["PostActionFilterFuncAsync"]
                    as Func<Task>;

                await func?.Invoke();
            }

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        private static void AddUpdateRateLimitingSuccessHeaders(HttpActionExecutedContext context, RateLimitingResult result)
        {
            var successheaders = new Dictionary<string, string>()
            {
                {RateLimitHeaders.TokensRemaining, result.TokensRemaining.ToString()},
                {RateLimitHeaders.Limit, result.CacheKey.AllowedCallRate.ToString() }
            };

            var response = context.Response;
            foreach (var successheader in successheaders.Keys)
            {
                if (response.Headers.Contains(successheader))
                {
                    // KAZI revisit
                    var successHeaderValues = response.Headers.GetValues(successheader)?.ToList() ?? new List<string>();
                    successHeaderValues.Add(successheaders[successheader]);
                    context.Response.Headers.Remove(successheader);
                    response.Headers.Add(successheader, successHeaderValues);
                }
                else
                {
                    response.Headers.Add(successheader, new string[] { successheaders[successheader] });
                }
            }
        }
    }
}