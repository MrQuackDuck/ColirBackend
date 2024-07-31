using Colir.Communication.Enums;
using Colir.Communication.ResponseModels;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs;

/// <summary>
/// Abstraction over <see cref="Hub"/> that adds "Results API"-like methods
/// </summary>
public abstract class ColirHub : Hub
{
    /// <summary>
    /// Returns a success result with content
    /// </summary>
    /// <param name="content">Content of the response</param>
    protected SignalRHubResult Success(object content)
    {
        return new SuccessWithContentHubResult(content);
    }
    
    /// <summary>
    /// Overload of <see cref="Success(object)"/> that takes no arguments
    /// <returns>Success <see cref="SignalRHubResult"/> without any content</returns>
    /// </summary>
    protected SignalRHubResult Success()
    {
        return new SuccessHubResult();
    }

    /// <summary>
    /// Error result that indicates that an error occurred
    /// </summary>
    /// <param name="error">Error response object</param>
    protected SignalRHubResult Error(ErrorResponse error)
    {
        return new ErrorHubResult(error);
    }
}