using ServiceReference;
using System.Collections.Generic;
using System.Linq;

public class SoapService : ISoapService
{
    private readonly NQWSReceptionSoapClient _client;

    public SoapService()
    {
        _client = new NQWSReceptionSoapClient(
            NQWSReceptionSoapClient.EndpointConfiguration.NQWSReceptionSoap
        );
    }

    public async Task<List<ServiceTypeQueueInfo>> GetServiceTypeQueueInfoAsync(
        int serviceType,
        string host,
        int port
    )
    {
        var response = await _client.GetServiceTypeQueueInfoAsync(
            serviceType,
            host,
            port
        );

        return response.Body?.GetServiceTypeQueueInfoResult?.ToList()
               ?? new List<ServiceTypeQueueInfo>();
    }
}

