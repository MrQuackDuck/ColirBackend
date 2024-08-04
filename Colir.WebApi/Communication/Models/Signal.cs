namespace Colir.Communication.Models;

public class Signal
{
    public long IssuerId { get; set; }
    public string Data { get; set; }

    public Signal(long issuerId, string data)
    {
        IssuerId = issuerId;
        Data = data;
    }
}