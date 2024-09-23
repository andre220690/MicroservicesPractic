namespace Ozon.Route256.Practice.OrdersService.Models;

public class OrdersRequestDto
{
    public IEnumerable<string> Regions { get; set; }
    public OrderState OrderState { get; set; }
    public int Pagination { get; set; }
    public int TypeSorted { get; set; }
    public int FieldSorted { get; set; }
}
