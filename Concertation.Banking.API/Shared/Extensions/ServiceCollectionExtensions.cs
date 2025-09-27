using System.Reflection;
using Concertation.Banking.API.Features.Accounts.Handlers;
using Concertation.Banking.API.Features.Auth.Handlers;
using Concertation.Banking.API.Features.Transactions.Handlers;
using Concertation.Banking.API.Features.Transactions.Services;
using Concertation.Banking.API.Features.Users.UpdateUserProfile;
using Concertation.Banking.API.Infrastructure.Redis;
using Concertation.Banking.API.Infrastructure.Services;
using Concertation.Banking.API.Shared.Interfaces;
using FluentValidation;
using StackExchange.Redis;

namespace Concertation.Banking.API.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        Assembly assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddScoped<PinService>();
        services.AddScoped<ValidateService>();
        services.AddScoped<IRefreshTokenService, RedisRefreshTokenService>();
        services.AddScoped<CheckUserRoleHandler>();
        services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
        services.AddScoped<ITransferIdGenerator, TransferIdGenerator>();
        services.AddScoped<BalanceHistoryService>();
        services.AddHttpClient<IKeycloakAuthService, KeycloakAuthClient>();
        services.AddScoped<PendingTransactionHandler>();
        services.AddScoped<PendingTransferHandler>();
        services.AddScoped<ConfirmTransactionHandler>();
        services.AddScoped<UpdateUserProfileHandler>();
        services.AddScoped<ConfirmUpdateUserProfileHandler>();
        services.AddScoped<TransactionFactory>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IPendingTransactionRedisStore, PendingTransactionRedisStore>();
        services.AddScoped<IPendingUpdateUserRedisStore, PendingUpdateUserRedisStore>();

        services.AddValidatorsFromAssemblyContaining<Program>();
        //services.AddValidatorsFromAssemblyContaining<TransferFundsRequestValidator>(ServiceLifetime.Scoped);
        //services.AddValidatorsFromAssemblyContaining<SetPinRequestValidator>(ServiceLifetime.Scoped);
        //services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>(ServiceLifetime.Scoped);
        //services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>(ServiceLifetime.Scoped);

        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString)
    {
        var redis = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        return services;
    }
}
