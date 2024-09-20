using System;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Repository.Models.Filter;

public record OrderFilter(
    string[] regions,
    OrderStateEnum orderState,
    int pagination,
    TypeSorted typeSorted,
    FieldSorted fieldSorted
);
