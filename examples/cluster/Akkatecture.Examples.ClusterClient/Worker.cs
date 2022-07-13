using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Hosting;
using Akkatecture.Examples.Domain.Model.UserAccount;
using Akkatecture.Examples.Domain.Model.UserAccount.Commands;
using Microsoft.Extensions.Hosting;

namespace Akkatecture.Examples.ClusterClient;

public class Worker : BackgroundService
{
    private readonly ActorRegistry _actorRegistry;

    public Worker(ActorRegistry actorRegistry)
    {
        _actorRegistry = actorRegistry;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Must get the region here since the registration isn't completed when the constructor is called
        var aggregateProxy = _actorRegistry.Get<ShardRegion<UserAccountAggregateManager, UserAccountAggregate, UserAccountId>>();
        
        Console.WriteLine("Press Enter To Create A Random User Account, or Q to quit.");
            
        var key = Console.ReadLine();
        var quit = key?.ToUpper() == "Q";
        
        while (!quit)
        {
            //Generate random, new UserAccount
            var aggregateId = UserAccountId.New;
            var randomUserAccountName = Guid.NewGuid().ToString();
            var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, randomUserAccountName);
                
            //Send the command
            aggregateProxy.Tell(createUserAccountCommand);

            Console.WriteLine($"CreateUsrAccountCommand: Id={createUserAccountCommand.AggregateId}; Name={createUserAccountCommand.Name} Sent.");

            Console.WriteLine("Press Enter To Create Another Random User Account, or Q to quit.");
            key = Console.ReadLine();
            quit = key?.ToUpper() == "Q";
        }

        return Task.CompletedTask;
    }
}