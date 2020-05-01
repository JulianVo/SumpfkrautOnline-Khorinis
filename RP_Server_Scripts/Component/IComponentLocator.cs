using System;

namespace RP_Server_Scripts.Component
{
    public interface IComponentLocator<in TOwner>
    {
        object GetComponent(TOwner ownerInstance);

        bool TryGetComponent(TOwner ownerInstance, out object component);

        Type SupportedType { get; }
    }
}
