using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.AutoFacManager
{
    public static class AutofacExtend
    {
        public static void UseCustomConfigureContainer(this ContainerBuilder containerBuilder)
        {
            var baseRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var filePaths = Directory.GetFiles(baseRootPath, "*.dll", SearchOption.TopDirectoryOnly);

            var assemblies = filePaths.Select(Assembly.LoadFrom).Distinct().ToArray();

            containerBuilder.BuildSingleton(assemblies).BuildScope(assemblies);

        }

        /// <summary>
        /// 注册单例服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static ContainerBuilder BuildSingleton(this ContainerBuilder builder, Assembly[] assemblies)
        {
            var singletonType = typeof(ISingletonService);
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(type => singletonType.IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract)
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .SingleInstance();

            return builder;
        }

        /// <summary>
        /// 注册作用域服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private static ContainerBuilder BuildScope(this ContainerBuilder builder, Assembly[] assemblies)
        {
            var scopeType = typeof(IScopeService);
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(type => scopeType.IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract)
                   .AsSelf()
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            return builder;
        }
    }
}
