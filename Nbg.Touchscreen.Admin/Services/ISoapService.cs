using ServiceReference;

public interface ISoapService
{
    Task<List<ServiceTypeQueueInfo>> GetServiceTypeQueueInfoAsync(
        int serviceType,
        string host,
        int port
    );
}

