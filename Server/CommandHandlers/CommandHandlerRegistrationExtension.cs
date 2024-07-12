using Server.Utilities.Constants;

namespace Server.CommandHandlers;

public static class CommandHandlerRegistrationExtension
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<EndCommandHandler>();
        services.AddScoped<AlertCommandHandler>();
        services.AddScoped<SendCommandHandler>();
        //services.AddScoped<DefaultCommandHandler>();

        services.AddScoped(sp =>
        {
            var registry = new CommandHandlerRegistry();

            registry.Register(AppConstants.EndCommand, () => sp.GetRequiredService<EndCommandHandler>());
            registry.Register(AppConstants.AlertCommand, () => sp.GetRequiredService<AlertCommandHandler>());
            registry.Register(AppConstants.SendCommand, () => sp.GetRequiredService<SendCommandHandler>());

            return registry;
        });

        return services;
    }
}
