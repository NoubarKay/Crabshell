using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Crabshell.Admin;

public static class AdminExtensions
{
    /// <summary>
    /// The Crabshell.Admin assembly — pass to AddAdditionalAssemblies in MapRazorComponents
    /// and AdditionalAssemblies in the Router component.
    /// </summary>
    public static Assembly Assembly => typeof(AdminExtensions).Assembly;

    /// <summary>
    /// Makes the Crabshell admin UI available. Admin pages are served under /admin by default.
    /// Pass a custom path to rewrite incoming requests to /admin (e.g. "/cms" → "/admin").
    /// If <paramref name="policy"/> is provided, all requests to the admin path are checked
    /// against that ASP.NET Core authorization policy. If no policy is provided the admin is
    /// publicly accessible.
    /// </summary>
    public static WebApplication MapCrabshellAdmin(this WebApplication app, string path = "/admin", string? policy = null)
    {
        var normalizedPath = path.TrimEnd('/');

        app.Use(async (ctx, next) =>
        {
            var requestPath = ctx.Request.Path;

            // Rewrite custom path → /admin
            if (normalizedPath != "/admin" &&
                requestPath.StartsWithSegments(normalizedPath, out var remainder))
            {
                ctx.Request.Path = "/admin" + remainder;
                requestPath = ctx.Request.Path;
            }

            // Enforce policy if configured
            if (policy is not null && requestPath.StartsWithSegments("/admin"))
            {
                var auth = ctx.RequestServices.GetRequiredService<IAuthorizationService>();
                var result = await auth.AuthorizeAsync(ctx.User, policy);
                if (!result.Succeeded)
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await next();
        });

        return app;
    }
}