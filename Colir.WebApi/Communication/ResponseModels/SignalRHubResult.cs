using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

/// <summary>
/// Base SignalR result class
/// </summary>
public abstract class SignalRHubResult
{
    public SignalrResultType ResultType { get; set; }
    
    public SignalRHubResult(SignalrResultType resultType)
    {
        ResultType = resultType;
    }
}