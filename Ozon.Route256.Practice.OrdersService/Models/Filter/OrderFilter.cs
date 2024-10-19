namespace Ozon.Route256.Practice.OrdersService.Models.Filter;

public record OrderFilter(
    string[] regions,
    OrderStateEnum orderState,
    int pagination,
    TypeSorted typeSorted,
    FieldSorted fieldSorted
);
