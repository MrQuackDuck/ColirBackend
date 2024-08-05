using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

public class ErrorHubResult : SignalRHubResult
{
    public ErrorResponse Error { get; }

    public ErrorHubResult(ErrorResponse error) : base(SignalrResultType.Error)
    {
        Error = error;
    }
}