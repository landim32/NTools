using Core.Domain;
using NTools.Domain.Impl.Core;
using NTools.Domain.Impl.Services;
using NTools.Domain.Interfaces.Core;
using NTools.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using NTools.Domain;
using System.Configuration;

namespace NTools.Application
{
    public static class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if(scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, string connection, bool scoped = true)
        {
            injectDependency(typeof(ILogCore), typeof(LogCore), services, scoped);

            injectDependency(typeof(IFileService), typeof(FileService), services, scoped);
            injectDependency(typeof(IMailerSendService), typeof(MailerSendService), services, scoped);

        }
    }
}
