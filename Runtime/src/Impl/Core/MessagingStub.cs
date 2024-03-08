using RGN.ImplDependencies.Core.Messaging;
using System;
using RGN.ImplDependencies.Core;

namespace RGN.Impl.Firebase.Core
{
    public sealed class MessagingStub : IMessaging, IImplStub
    {
        public event Action<object, ITokenReceivedEventArgs> TokenReceived { add { } remove { } }

        public event Action<object, IMessageReceivedEventArgs> MessageReceived { add { } remove { } }
    }
}
