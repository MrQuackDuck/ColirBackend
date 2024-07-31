using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

public class SuccessWithContentHubResult : SignalRHubResult
{
    public object? Content { get; }

    public SuccessWithContentHubResult(object content) : base(SignalrResultType.Success)
    {
        Content = content;
    }
}