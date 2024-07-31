using Colir.Communication.Enums;

namespace Colir.Communication.ResponseModels;

public class ErrorResponse
{
    public ErrorCode ErrorCode { get; init; }
    public string ErrorCodeAsString { get; }
    public string? Details { get; }

    public ErrorResponse(ErrorCode code)
    {
        ErrorCode = code;
        ErrorCodeAsString = ErrorCode.ToString();
    }
    
    public ErrorResponse(ErrorCode code, string details)
    {
        ErrorCode = code;
        ErrorCodeAsString = ErrorCode.ToString();
        Details = details;
    }
}