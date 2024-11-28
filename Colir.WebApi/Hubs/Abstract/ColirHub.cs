using System.Collections.Concurrent;
using Colir.Communication.ResponseModels;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs.Abstract;

/// <summary>
/// Abstraction over <see cref="Hub"/> that adds "Results API"-like methods and other useful methods
/// </summary>
public abstract class ColirHub : Hub
{
    /// <summary>
    /// Dictionary of connected clients (used to implement custom <see cref="Disconnect"/> method)
    /// The key is a connection ID and the value is a <see cref="HubCallerContext"/>
    /// </summary>
    protected static readonly ConcurrentDictionary<string, HubCallerContext> ConnectedClients = new();

    /// <summary>
    /// Adds a client to the <see cref="ConnectedClients"/> dictionary
    /// </summary>
    public override Task OnConnectedAsync()
    {
        ConnectedClients.TryAdd(Context.ConnectionId, Context);
        return base.OnConnectedAsync();
    }

    /// <summary>
    /// Removes a client from the <see cref="ConnectedClients"/> dictionary
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedClients.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

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
    /// <param name="disconnect">Is user supposed to be disconnected after receiving an error</param>
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
    /// <returns>A boolean indicating if the model is valid</returns>
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
    /// Disconnects the user from the hub by connection ID
    /// </summary>
    protected static void Disconnect(string connectionId)
    {
        if (ConnectedClients.TryGetValue(connectionId, out var clientContext))
        {
            clientContext.Abort();
        }
    }
}