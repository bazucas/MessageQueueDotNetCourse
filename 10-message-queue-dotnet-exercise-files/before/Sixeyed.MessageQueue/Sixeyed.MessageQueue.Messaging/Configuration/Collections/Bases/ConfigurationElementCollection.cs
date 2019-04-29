using System.Collections.Generic;
using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public abstract class ConfigurationElementCollection<TElement> : ConfigurationElementCollection, IEnumerable<TElement>
        where TElement : ConfigurationElement, new()
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public TElement this[int index]
        {
            get { return (TElement)BaseGet(index); }
        }

        public new TElement this[string key]
        {
            get { return (TElement)BaseGet(key); }
        }

        public new IEnumerator<TElement> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return BaseGet(i) as TElement;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        protected sealed override object GetElementKey(ConfigurationElement element)
        {
            return GetElementKey((TElement) element);
        }

        protected abstract object GetElementKey(TElement element);
    }
}