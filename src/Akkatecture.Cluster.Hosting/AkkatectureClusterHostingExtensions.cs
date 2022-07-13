using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;

namespace Akkatecture.Cluster.Hosting;

public static class AkkatectureClusterHostingExtensions
{
    public static void WithClusteredAggregate(this AkkaConfigurationBuilder builder)
    {
        var clusterSharding = ClusterSharding.Get(actorSystem);
        var clusterShardingSettings = clusterSharding.Settings;

        var aggregateManagerProps = Props.Create<TAggregateManager>();
            
        var shardRef = clusterSharding.Start(
            typeof(TAggregateManager).Name,
            Props.Create(() => new ClusterParentProxy(aggregateManagerProps, true)),
            clusterShardingSettings,
            new MessageExtractor<TAggregate, TIdentity>(numberOfShards)
        );

        
        var b = builder
            .WithShardRegion<object>(typeof(TAggregateManager).Name, 
                _ =>  Props.Create(() => new ClusterParentProxy(aggregateManagerProps, true)))
    }
}