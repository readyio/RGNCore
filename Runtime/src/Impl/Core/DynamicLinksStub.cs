using System;
using RGN.ImplDependencies.Core;
using RGN.ImplDependencies.Core.DynamicLinks;

namespace RGN.Impl.Firebase.Core
{
    public class DynamicLinksStub : IDynamicLinks, IImplStub
    {
        public event Action<object, IDynamicLinkReceivedEventArgs> DynamicLinkReceived { add { } remove { } }
    }
}
