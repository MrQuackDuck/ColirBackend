﻿namespace Colir.Communication.RequestModels.Chat;

public class GetLastMessagesModel
{
    public int Count { get; set; }
    public int SkipCount { get; set; }
    public string RoomGuid { get; set; } = default!;
}