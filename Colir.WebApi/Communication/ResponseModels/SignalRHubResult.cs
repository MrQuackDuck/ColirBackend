using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

/// <summary>
/// Base SignalR result class
/// </summary>
public abstract class SignalRHubResult
{
    public SignalRResultType ResultType { get; set; }

    public SignalRHubResult(SignalRResultType resultType)
    {
        ResultType = resultType;
    }
}