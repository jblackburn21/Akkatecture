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
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Core
{
    public class MessageExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator> : HashCodeMessageExtractor
        where TAggregateSagaManager : IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>, new()
    {
        private TSagaLocator SagaLocator { get; }
        public MessageExtractor(int maxNumberOfShards) 
            : base(maxNumberOfShards)
        {
            SagaLocator = new TSagaLocator();
        }

        public override string EntityId(object message)
        {
            switch(message)
            {
                case IDomainEvent e: return SagaLocator.LocateSaga(e).Value;
                case ShardRegion.StartEntity start: return start.EntityId; // TODO: Needed for remember entities, but need to verify what to return
            }

            return null;
        }
    }
    public class MessageExtractor<TAggregate, TIdentity> : HashCodeMessageExtractor
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public MessageExtractor(int maxNumberOfShards) 
            : base(maxNumberOfShards)
        {
        }

        public override string EntityId(object message)
        {
            switch(message)
            {
                case ICommand<TAggregate, TIdentity> cmd: return cmd.AggregateId.Value;
                case ShardRegion.StartEntity start: return start.EntityId; // TODO: Needed for remember entities, but need to verify what to return
            }

            return null;
        }
    }
}


// public sealed class MessageExtractor : HashCodeMessageExtractor
// {
//     public MessageExtractor() : base(maxNumberOfShards: 100) { }
//
//     public string EntityId(object message) 
//     {
//         switch(message)
//         {
//             case ShardEnvelope e: return e.EntityId;
//             case ShardRegion.StartEntity start: return start.EntityId;
//         }
//     } 
//     public object EntityMessage(object message) => (message as ShardEnvelope)?.Message ?? message;
// }