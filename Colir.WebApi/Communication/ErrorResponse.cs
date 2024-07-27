namespace Colir.Communication;

public class ErrorResponse
{
    public ErrorCode ErrorCode { get; init; }
    public string ErrorCodeAsString { get; }

    public ErrorResponse(ErrorCode code)
    {
        ErrorCode = code;
        ErrorCodeAsString = ErrorCode.ToString();
    }
}