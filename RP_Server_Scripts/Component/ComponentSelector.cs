using System;
using System.Collections.Generic;
using System.Linq;

namespace RP_Server_Scripts.Component
{
    public sealed class ComponentSelector<TOwner>
    {
        private readonly Dictionary<Type, IComponentLocator<TOwner>> _Locators;

        public ComponentSelector(IEnumerable<IComponentLocator<TOwner>> locators)
        {
            if (locators == null)
            {
                throw new ArgumentNullException(nameof(locators));
            }

            _Locators = locators.ToDictionary(loc => loc.SupportedType);
        }

        public object GetComponent(Type componentType, TOwner ownerInstance)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }

            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            if (!_Locators.TryGetValue(componentType, out IComponentLocator<TOwner> locator))
            {
                throw new ComponentNotFoundException(
                    $"A component of type '{componentType.Name}' was not found for type '{typeof(TOwner).Name}'");
            }

            return locator.GetComponent(ownerInstance);
        }

        public TComponent GetComponent<TComponent>(TOwner ownerInstance)
        {
            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            if (!_Locators.TryGetValue(typeof(TComponent), out IComponentLocator<TOwner> locator))
            {
                throw new ComponentNotFoundException(
                    $"A component of type '{typeof(TComponent).Name}' was not found for type '{typeof(TOwner).Name}'");
            }

            return (TComponent)locator.GetComponent(ownerInstance);
        }

        public bool TryGetComponent(Type componentType, TOwner ownerInstance, out object component)
        {
            if (componentType == null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }

            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            if (!_Locators.TryGetValue(componentType, out IComponentLocator<TOwner> locator))
            {
                component = null;
                return false;
            }

            if (!locator.TryGetComponent(ownerInstance, out component))
            {
                return false;
            }

            return true;
        }

        public bool TryGetComponent<TComponent>(TOwner ownerInstance, out TComponent component)
        {
            if (ownerInstance == null)
            {
                throw new ArgumentNullException(nameof(ownerInstance));
            }

            if (!_Locators.TryGetValue(typeof(TComponent), out IComponentLocator<TOwner> locator))
            {
                component = default(TComponent);
                return false;
            }

            if (!locator.TryGetComponent(ownerInstance, out object tempComponent))
            {
                component = default(TComponent);
                return false;
            }

            component = (TComponent)tempComponent;
            return true;
        }
    }
}
