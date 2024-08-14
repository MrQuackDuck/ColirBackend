using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

public class SuccessHubResult : SignalRHubResult
{
    public SuccessHubResult() : base(SignalRResultType.Success)
    {
    }
}