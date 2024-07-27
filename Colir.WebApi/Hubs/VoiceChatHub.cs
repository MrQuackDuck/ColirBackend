using Colir.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs;

[Authorize]
public class VoiceChatHub : Hub<IVoiceChatHub>
{
    public async Task Connect(string roomGuid)
    {
        throw new NotImplementedException();
    }

    public async Task Join()
    {
        throw new NotImplementedException();
    }

    public async Task Leave()
    {
        throw new NotImplementedException();
    }

    public async Task MuteSelf()
    {
        throw new NotImplementedException();
    }

    public async Task UnmuteSelf()
    {
        throw new NotImplementedException();
    }

    public async Task DefeanSelf()
    {
        throw new NotImplementedException();
    }

    public async Task UndefeanSelf()
    {
        throw new NotImplementedException();
    }

    public async Task SendVoiceSignal(string audioData)
    {
        throw new NotImplementedException();
    }

    public async Task EnableVideo()
    {
        throw new NotImplementedException();
    }

    public async Task DisableVideo()
    {
        throw new NotImplementedException();
    }

    public async Task SendVideo(string videoData)
    {
        throw new NotImplementedException();
    }

    public async Task EnableStream()
    {
        throw new NotImplementedException();
    }

    public async Task DisableStream()
    {
        throw new NotImplementedException();
    }

    public async Task SendStreamPicture(string pictureData)
    {
        throw new NotImplementedException();
    }
}