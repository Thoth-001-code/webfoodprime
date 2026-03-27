using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace webfoodprime.IntegrationTests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Allow test to pass role via header "X-User-Role" and user id via "X-User-Id"
            var headers = Request.Headers;
            var role = headers.ContainsKey("X-User-Role") ? headers["X-User-Role"].ToString() : "TestRole";
            var userId = headers.ContainsKey("X-User-Id") ? headers["X-User-Id"].ToString() : "test-user-id";

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "test-user"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
