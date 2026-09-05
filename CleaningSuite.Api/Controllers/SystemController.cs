using CleaningSuite.Application.Tenants.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuite.Api.Controllers;

/// <summary>Cross-tenant onboarding. Provisioner token (master realm) only.</summary>
[ApiController]
[Route("api/v1/system/tenants")]
[Authorize]
public class SystemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly string _provisionerRealm;

    public SystemController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _provisionerRealm = configuration["Auth:Keycloak:ProvisionerRealm"] ?? "master";
    }

    [HttpPost]
    public async Task<IActionResult> Provision(ProvisionTenantCommand command, CancellationToken ct)
    {
        var realm = HttpContext.Items["Realm"] as string ?? "";
        if (realm != _provisionerRealm)
            return Forbid();

        var result = await _mediator.Send(command, ct);
        return result.AlreadyExisted
            ? Conflict(new { slug = result.Slug, title = "Tenant already registered" })
            : StatusCode(StatusCodes.Status201Created, result);
    }
}
