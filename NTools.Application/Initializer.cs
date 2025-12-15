using Microsoft.Extensions.DependencyInjection;
using NTools.Domain.Services;
using NTools.Domain.Services.Interfaces;
using System;

namespace NTools.Application
{
    public static class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if (scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, string connection, bool scoped = true)
        {
            services.AddHttpClient();

            injectDependency(typeof(IFileService), typeof(FileService), services, scoped);
            injectDependency(typeof(IMailerSendService), typeof(MailerSendService), services, scoped);

        }
    }
}
