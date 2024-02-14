using RGN.ImplDependencies.Core.Messaging;
using System;

namespace RGN.Impl.Firebase.Core
{
    public sealed class MessagingStub : IMessaging
    {
        public event Action<object, ITokenReceivedEventArgs> TokenReceived { add { } remove { } }

        public event Action<object, IMessageReceivedEventArgs> MessageReceived { add { } remove { } }
    }
}
