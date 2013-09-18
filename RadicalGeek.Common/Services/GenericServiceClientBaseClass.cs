using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RadicalGeek.Common.Services
{
    public abstract class GenericServiceClientBaseClass<TInterface> : IDisposable
    {
        protected readonly TInterface Service;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (Service is IDisposable)
                    ((IDisposable)Service).Dispose();
        }

        ~GenericServiceClientBaseClass()
        {
            Dispose(false);
        }

        private static readonly Dictionary<EndpointAddress, ChannelFactory<TInterface>> channelFactoryCache = new Dictionary<EndpointAddress, ChannelFactory<TInterface>>();

        protected GenericServiceClientBaseClass(Func<Binding> bindingCreator, EndpointAddress endpointAddress)
        {
            if (!channelFactoryCache.ContainsKey(endpointAddress))
                channelFactoryCache.Add(endpointAddress, new ChannelFactory<TInterface>(bindingCreator(), endpointAddress));
            Service = channelFactoryCache[endpointAddress].CreateChannel();
        }
    }
}