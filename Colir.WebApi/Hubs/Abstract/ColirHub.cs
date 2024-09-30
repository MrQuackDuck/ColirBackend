using System.Collections.Concurrent;
using System.Reflection;
using Colir.Communication.ResponseModels;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs.Abstract;

/// <summary>
/// Abstraction over <see cref="Hub"/> that adds "Results API"-like methods and other useful methods
/// </summary>
public abstract class ColirHub : Hub
{
    /// <summary>
    /// Returns a success result with content
    /// </summary>
    /// <param name="content">Content of the response</param>
    protected static SignalRHubResult Success(object content)
    {
        return new SuccessWithContentHubResult(content);
    }

    /// <summary>
    /// Overload of <see cref="Success(object)"/> that takes no arguments
    /// <returns>Success <see cref="SignalRHubResult"/> without any content</returns>
    /// </summary>
    protected static SignalRHubResult Success()
    {
        return new SuccessHubResult();
    }

    /// <summary>
    /// Error result that indicates that an error occurred
    /// </summary>
    /// <param name="error">Error response object</param>
    /// <param name="disconnect">Is user supposted to be disconnected after receiving an error</param>
    protected SignalRHubResult Error(ErrorResponse error, bool disconnect = false)
    {
        try
        {
            return new ErrorHubResult(error);
        }
        finally
        {
            if (disconnect) Context.Abort();
        }
    }

    /// <summary>
    /// Validates that each non-nullable property of a model is actually not set to null
    /// </summary>
    /// <param name="model">A model to validate</param>
    /// <returns>A boolean is a model valid</returns>
    protected static bool IsModelValid(object? model)
    {
        // Model itself should not be null
        if (model == null)
            return false;

        // Get the type of the model
        var modelType = model.GetType();

        // Iterate over each property in the model
        foreach (var property in modelType.GetProperties())
        {
            // Check if the property is non-nullable
            if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                // Check if the property value is null
                var value = property.GetValue(model);
                if (value == null)
                    return false; // A non-nullable property is null
            }
        }

        return true;
    }

    /// <summary>
    /// Gets all connected clients
    /// </summary>
    protected ConcurrentDictionary<string, HubConnectionContext> GetConnectedClients()
    {
        try
        {
#pragma warning disable S3011
            var lifetimeManagerPropInfo = this.Clients.All.GetType().GetField("_lifetimeManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var lifetimeManager = lifetimeManagerPropInfo!.GetValue(this.Clients.All);
            var connectionsPropInfo = lifetimeManager!.GetType().GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance);
            var connections = connectionsPropInfo!.GetValue(lifetimeManager);
            var connectionsInnerPropInfo = connections!.GetType().GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance);
            return (ConcurrentDictionary<string, HubConnectionContext>) connectionsInnerPropInfo!.GetValue(connections)!;
#pragma warning restore S3011
        }
        catch (ObjectDisposedException)
        {
            return new();
        }
    }
}