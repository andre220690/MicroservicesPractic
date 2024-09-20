using System.Collections.Concurrent;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Repository.Models;

public class InMemoryStorage
{
    public readonly ConcurrentDictionary<long, OrderDto> Orders = new(2, 10);

    public InMemoryStorage()
    {
        FakeOrders();
    }

    private void FakeOrders()
    {
        var regions = new[] { "Moscow", "StPetersburg", "Novosibirsk" };

        var orders = Enumerable.Range(1, 10000)
                .Select(r => new OrderDto(
                    r,
                    Faker.RandomNumber.Next(1, 10),
                    GetRandomDouble(1, 10),
                    GetRandomDouble(1, 10),
                    Faker.RandomNumber.Next(1, 3),
                    GetRandomDateTime(),
                    regions[Faker.RandomNumber.Next(0, 2)],
                    (OrderStateEnum)Faker.RandomNumber.Next(0, 5),
                    Faker.RandomNumber.Next(1, 50),
                    Faker.Name.FullName(),
                    Faker.Address.StreetName(),
                    Faker.Phone.Number()
            ));

        foreach(var order in orders)
        {
            Orders[order.id] = order;
        }
    }

    private double GetRandomDouble(double minimum, double maximum)
    {
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    private DateTime GetRandomDateTime()
    {
        Random random = new Random();
        return DateTime.Now.AddSeconds(random.Next(1, 31500000) * -1);
    }
}
