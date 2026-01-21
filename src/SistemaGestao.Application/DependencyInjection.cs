using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestao.Application.Mappings;
using SistemaGestao.Application.Services;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Validations;

namespace SistemaGestao.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IValidator<Cliente>, ClienteValidator>();
            return services;
        }
    }
}