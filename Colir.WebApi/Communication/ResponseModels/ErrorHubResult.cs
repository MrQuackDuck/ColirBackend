using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

public class ErrorHubResult : SignalRHubResult
{
    public ErrorResponse Error { get; }

    public ErrorHubResult(ErrorResponse error) : base(SignalRResultType.Error)
    {
        Error = error;
    }
}