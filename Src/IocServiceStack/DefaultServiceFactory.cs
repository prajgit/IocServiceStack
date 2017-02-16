﻿#region License
// Copyright (c) 2016-2017 Rajeswara Rao Jinaga
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace IocServiceStack
{
    using System;
    using System.Reflection;

    public class DefaultServiceFactory : BaseServiceFactory, IRootServiceFactory
    {
        private ServiceNotifier _notifier;
        private ServiceCompiler _compiler;

        public DefaultServiceFactory(string[] namespaces, Assembly[] assemblies, bool strictMode, IDependencyFactory dependencyFactory, ISharedFactory sharedFactory)
            : base(namespaces, assemblies, strictMode, dependencyFactory, sharedFactory)
        {

        }

        public virtual BaseServiceInfo GetServiceInfo(Type contractType, string serviceName)
        {
            if (!ServicesMapTable.Contains(contractType))
                throw ExceptionHelper.ThrowServiceNotRegisteredException(contractType.Name);

            if (string.IsNullOrEmpty(serviceName))
            {
                return ServicesMapTable[contractType];
            }
            {
                return ServicesMapTable[contractType, serviceName];
            }
        }

        public virtual BaseServiceInfo GetServiceInfo(Type contractType)
        {
            return GetServiceInfo(contractType, null);
        }

        public virtual T Create<T>(BaseServiceInfo serviceMeta) where T : class
        {
            if (serviceMeta != null)
            {
                if (serviceMeta.Activator == null)
                {
                    Type interfaceType = typeof(T);
                    //Compile
                    _compiler.Compile<T>(interfaceType, serviceMeta, CreateConstructorExpression);
                }
                return serviceMeta.Activator.CreateInstance<T>();
            }
            return default(T);
        }

        public object Create(Type contractType, BaseServiceInfo serviceMeta)
        {
            if (serviceMeta != null)
            {
                if (serviceMeta.Activator == null)
                {
                    //Compile
                    _compiler.Compile<object>(contractType, serviceMeta, CreateConstructorExpression);
                }
                return serviceMeta.Activator.CreateInstance<object>();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            ContractObserver = new DefaultContractWorkObserver();
            _notifier = new ServiceNotifier();
            _compiler = new ServiceCompiler(_notifier);

            //set contract observer to SharedFactory
            SharedFactory.ContractObserver = ContractObserver;

            ContractObserver.OnUpdate((type) =>
            {
                _notifier.SendUpdate(type);
            });

        }
        public IDependencyFactory GetDependencyFactory(string factoryName)
        {
            return ServiceProviderHelper.GetDependencyFactory(DependencyFactory, factoryName);
        }

        public ISharedFactory GetSharedFactory()
        {
            return SharedFactory;
        }

        public IRootContainer Add<TC>(Func<TC> expression) where TC: class
        {
            return AddInternal<TC>(expression, null);
        }

        public IRootContainer Add<TC>(Func<TC> expression, string serviceName) where TC : class
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                ExceptionHelper.ThrowArgumentNullException(nameof(serviceName));
            }

            return AddInternal<TC>(expression, serviceName);
        }

        public IRootContainer Replace<TC>(Func<TC> expression) where TC : class
        {
            return ReplaceInternal<TC>(expression, null);
        }

        public IRootContainer Replace<TC>(Func<TC> expression, string serviceName) where TC : class
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                ExceptionHelper.ThrowArgumentNullException(nameof(serviceName));
            }

            return ReplaceInternal<TC>(expression, serviceName);
        }

        public IRootContainer Add<TC, TS>()
           where TC : class
           where TS : TC
        {
            AddService<TC, TS>();
            return this;
        }

        public IRootContainer Add<TC, TS>(string serviceName)
            where TC : class
            where TS : TC
        {
            AddService<TC, TS>(serviceName);
            return this;
        }

        public IRootContainer Add<T>(Type service) where T : class
        {
            AddService<T>(service);
            return this;
        }

        public IRootContainer Add<T>(Type service, string serviceName) where T : class
        {
            AddService<T>(service, serviceName);
            return this;
        }

        public IRootContainer AddSingleton<TC, TS>()
            where TC : class
            where TS : TC
        {
            AddSingletonService<TC, TS>();
            return this;
        }

        public IRootContainer AddSingleton<TC, TS>(string serviceName)
            where TC : class
            where TS : TC
        {
            AddSingletonService<TC, TS>(serviceName);
            return this;
        }

        public IRootContainer Replace<TC, TS>()
            where TC : class
            where TS : TC
        {
            ReplaceService<TC, TS>();
            return this;
        }

        public IRootContainer Replace<TC, TS>(string serviceName)
            where TC : class
            where TS : TC
        {
            ReplaceService<TC, TS>(serviceName);
            return this;
        }

        public IRootContainer Replace<T>(Type service) where T : class
        {
            ReplaceService<T>(service);
            return this;
        }

        public IRootContainer Replace<T>(Type service, string serviceName) where T : class
        {
            ReplaceService<T>(service, serviceName);
            return this;
        }

        public IRootContainer ReplaceSingleton<TC, TS>()
            where TC : class
            where TS : TC
        {
            ReplaceSingletonService<TC, TS>();
            return this;
        }

        public IRootContainer ReplaceSingleton<TC, TS>(string serviceName)
            where TC : class
            where TS : TC
        {
            ReplaceSingletonService<TC, TS>(serviceName);
            return this;
        }

        private IRootContainer AddInternal<TC>(Func<TC> serviceAction, string serviceName) where TC : class
        {
            Type interfaceType = typeof(TC);
            var serviceMeta = new ServiceInfo<TC,TC>(serviceAction, BaseServiceInfo.GetDecorators(interfaceType), serviceName);

            ServicesMapTable.Add(interfaceType, serviceMeta);

            //send update to observer
            ContractObserver.Update(interfaceType);

            return this;
        }

        private IRootContainer ReplaceInternal<TC>(Func<TC> expression, string serviceName) where TC : class
        {
            Type interfaceType = typeof(TC);
            var serviceMeta = new ServiceInfo<TC,TC>(expression, BaseServiceInfo.GetDecorators(interfaceType), serviceName);

            ServicesMapTable.AddOrReplace(interfaceType,serviceMeta);

            //send update to observer
            ContractObserver.Update(interfaceType);

            return this;
        }

      

        //private void Compile<T>(Type interfaceType, BaseServiceInfo serviceMeta) where T : class
        //{
        //    if (serviceMeta.Activator == null)
        //    {
        //        lock (serviceMeta.SyncObject)
        //        {
        //            if (serviceMeta.Activator == null)
        //            {
        //                var registrar = serviceMeta.InitNewRegister(interfaceType, _notifier);

        //                Func<T> serviceCreator  = serviceMeta.GetServiceInstanceCallback<T>();
        //                if (serviceCreator == null)
        //                {
        //                    var state = new ServiceState(); /*Root place for service state instance*/
        //                    Expression newExpression = CreateConstructorExpression(interfaceType, serviceMeta.ServiceType, registrar, state);

        //                    var blockExpression = BuildExpression(state, newExpression);

        //                    //Set Activator
        //                    serviceCreator = Expression.Lambda<Func<T>>(blockExpression).Compile();
        //                }
        //                serviceMeta.Activator = new ServiceActivator<T>(serviceCreator, serviceMeta.IsReusable);
        //            }
        //        }
        //    }
        //}

        //private Expression BuildExpression(ServiceState state, Expression returnValue)
        //{
        //    /*we do not need to build block if there are no parameters.
        //     * it means that there are no re-usable instances within constructor parameters.
        //     */
        //    if (!state.HasParameters())
        //    {
        //        return returnValue;
        //    }

        //    List<Expression> expressions = new List<Expression>(state.GetBinaryExpressions());
        //    expressions.Add(returnValue);

        //    BlockExpression blockExpr = Expression.Block(
        //      state.GetParameters(),
        //      expressions
        //    );

        //    return blockExpr;
        //}
    }
}
