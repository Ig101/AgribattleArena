using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectArena.Application
{
  public static class ApplicationRegistry
    {
        public static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}