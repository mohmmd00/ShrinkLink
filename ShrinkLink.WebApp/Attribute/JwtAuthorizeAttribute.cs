using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShrinkLink.WebApp.attribute
{
    public class JwtAuthorizeAttribute : Attribute , IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Cookies["jwt-token"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                {
                    { "Controller", "Home" },
                    { "Action", "LoginPage" }
                });
            }

            return Task.CompletedTask;
        }
    }
}
