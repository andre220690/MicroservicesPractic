namespace Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;

public interface IAddressRepository
{
    Task<string[]> GetRegions(CancellationToken token);
}
