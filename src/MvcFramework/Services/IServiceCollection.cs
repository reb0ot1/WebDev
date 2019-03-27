using System;
using System.Collections.Generic;
using System.Text;

namespace MvcFramework.Services
{
    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        T CreateInstance<T>();

        object CreateInstance(Type type);

        void AddService<T>(Func<T> p);
    }
}
