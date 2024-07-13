using Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Client.CommandHandlers;

internal static class CommandHandlerRegistrationExtension
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<EndCommandHandler>();
        services.AddScoped<AlertCommandHandler>();
        services.AddScoped<SendCommandHandler>();
        services.AddScoped<StreamCommandHandler>();

        services.AddScoped(sp =>
        {
            var registry = new CommandHandlerRegistry();

            registry.Register(AppConstants.EndCommand, () => sp.GetRequiredService<EndCommandHandler>());
            registry.Register(AppConstants.AlertCommand, () => sp.GetRequiredService<AlertCommandHandler>());
            registry.Register(AppConstants.SendCommand, () => sp.GetRequiredService<SendCommandHandler>());
            registry.Register(AppConstants.StreamCommand, () => sp.GetRequiredService<StreamCommandHandler>());

            return registry;
        });

        return services;
    }
}
