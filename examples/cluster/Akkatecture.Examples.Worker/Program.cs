// The MIT License (MIT)
//
// Copyright (c) 2018 - 2021 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Akkatecture.Clustering;
using Akkatecture.Clustering.Core;
using Akkatecture.Examples.Domain.Model.UserAccount;
using Microsoft.Extensions.Hosting;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var actorSystemName = "akkatecture";

        services.AddAkka(actorSystemName, (builder, sp) =>
        {
            builder
                .WithRemoting("localhost", 7001)
                .WithClustering(new ClusterOptions()
                {
                    Roles = new[] { "worker" },
                    SeedNodes = new[] { Address.Parse($"akka.tcp://{actorSystemName}@localhost:7000") }
                })
                .AddHoconFile("worker.conf")
                // Enable host for https://cmd.petabridge.com
                .AddPetabridgeCmd(cmd =>
                {
                    cmd.RegisterCommandPalette(ClusterCommands.Instance);
                    cmd.RegisterCommandPalette(ClusterShardingCommands.Instance);
                })
                .WithShardRegion<ShardRegion<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>>(
                    nameof(UserAccountAggregateManager),
                    _ => Props.Create(() => new ClusterParentProxy(Props.Create<UserAccountAggregateManager>(), true)),
                    new MessageExtractor<UserAccountAggregate, UserAccountId>(12),
                    new ShardOptions
                    {
                        Role = "worker",
                        // Doc: https://getakka.net/api/Akka.Cluster.Sharding.ClusterShardingSettings.html#Akka_Cluster_Sharding_ClusterShardingSettings_RememberEntities
                        RememberEntities = true,
                        StateStoreMode = StateStoreMode.DData
                    });
            // TODO: Implement this in Akkatecture.Cluster.Hosting
            // .WithActors((actorSystem, registry) =>
            // {
            //     ClusterFactory<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>
            //         .StartClusteredAggregate(actorSystem);
            // });
        });
    })
    .Build();

Console.WriteLine("Akkatecture.Examples.Workers Running");

await host.RunAsync();

Console.WriteLine("Akkatecture.Examples.Workers Exiting.");


public record ShardRegion<TAggregateManager, TAggregate, TId>();
