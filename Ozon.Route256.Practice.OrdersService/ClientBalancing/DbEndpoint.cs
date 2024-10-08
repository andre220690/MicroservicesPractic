namespace Ozon.Route256.Practice.OrdersService.ClientBalancing;

public record DbEndpoint(
    string HostAndPort,
    DbReplicaType DbReplica,
    int[] Buckets);