using RGN.ImplDependencies.Core.Messaging;
using System;
using RGN.ImplDependencies.Core;
using System.Threading.Tasks;

namespace RGN.Impl.Firebase.Core
{
    public sealed class MessagingStub : IMessaging, IImplStub
    {
        public event Action<object, ITokenReceivedEventArgs> TokenReceived { add { } remove { } }

        public event Action<object, IMessageReceivedEventArgs> MessageReceived { add { } remove { } }
        public Task TopicSubscribeAsync(string topic)
        {
            return Task.CompletedTask;
        }
        public Task TopicUnsubscribeAsync(string topic)
        {
            return Task.CompletedTask;
        }
        public Task RequestPermissionAsync()
        {
            return Task.CompletedTask;
        }
        public Task<string> GetTokenAsync()
        {
            return Task.FromResult(string.Empty);
        }
        public Task DeleteTokenAsync()
        {
            return Task.CompletedTask;
        }
    }
}
