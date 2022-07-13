﻿// The MIT License (MIT)
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

// using System;
// using System.IO;
// using System.Threading.Tasks;
// using Akka.Actor;
// using Akka.Configuration;
// using Akkatecture.Clustering.Configuration;

using System;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Remote.Hosting;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
        {
            var actorSystemName = "akkatecture";

            services.AddAkka(actorSystemName, (builder, sp) =>
            {
                builder
                    .WithRemoting("localhost", 7000)
                    .WithClustering(new ClusterOptions()
                    {
                        Roles = new[] { "seed" },
                        SeedNodes = new[] { Address.Parse($"akka.tcp://{actorSystemName}@localhost:7000") }
                    })
                    .AddHoconFile("seed.conf");
            });
        })
        .Build();

Console.WriteLine("Akkatecture.Examples.Seed Running");

await host.RunAsync();

Console.WriteLine("Akkatecture.Examples.Seed Exiting.");

// namespace Akkatecture.Examples.Seed
// {
//     public static class Program
//     {
//         public static async Task Main(string[] args)
//         {
//             //Get configuration file using Akkatecture's defaults as fallback
//             var path = Environment.CurrentDirectory;
//             var configPath = Path.Combine(path, "seed.conf"); 
//             var config = ConfigurationFactory.ParseString(File.ReadAllText(configPath))
//                 .WithFallback(AkkatectureClusteringDefaultSettings.DefaultConfig());
//             var clustername = config.GetString("akka.cluster.name");
//
//             //Create actor system
//             var actorSystem = ActorSystem.Create(clustername, config);
//
//             Console.WriteLine("Akkatecture.Examples.Seed Running");
//             
//             var quit = false;
//
//             while (!quit)
//             {
//                 Console.Write("\rPress Q to Quit.         ");
//                 var key = Console.ReadLine();
//                 quit = key?.ToUpper() == "Q";
//             }
//
//             //Shut down the local actor system
//             await actorSystem.Terminate();
//             Console.WriteLine("Akkatecture.Examples.Seed Exiting.");
//         }
//     }
//     
//     
// }
