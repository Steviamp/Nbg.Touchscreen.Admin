using Microsoft.AspNetCore.Mvc;
using ServiceReference;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class ServiceTypeQueueInfoController : ControllerBase
{
    private readonly ISoapService _soap;

    public ServiceTypeQueueInfoController(ISoapService soap)
    {
        _soap = soap;
    }

    /// GET /api/servicetypequeueinfo/1?host=127.0.0.1&port=8899
    [HttpGet("{serviceType}")]
    public async Task<ActionResult<List<ServiceTypeQueueInfo>>> Get(
        int serviceType,
        [FromQuery] string host,
        [FromQuery] int port
    )
    {
        var result = await _soap.GetServiceTypeQueueInfoAsync(
            serviceType,
            host,
            port
        );

        return Ok(result);
    }
}

