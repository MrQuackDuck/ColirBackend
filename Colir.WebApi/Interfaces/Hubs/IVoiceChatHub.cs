using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IVoiceChatHub
{
    SignalRHubResult GetVoiceChatUsers();
    
    Task<SignalRHubResult> Join();
    Task<SignalRHubResult> Leave();

    Task<SignalRHubResult> MuteSelf();
    Task<SignalRHubResult> UnmuteSelf();
    Task<SignalRHubResult> DefeanSelf();
    Task<SignalRHubResult> UndefeanSelf();
    Task<SignalRHubResult> SendVoiceSignal(string audioData);

    Task<SignalRHubResult> EnableVideo();
    Task<SignalRHubResult> DisableVideo();
    Task<SignalRHubResult> SendVideoSignal(string videoData);

    Task<SignalRHubResult> EnableStream();
    Task<SignalRHubResult> DisableStream();
    Task<SignalRHubResult> SendStreamSignal(string pictureData);

    SignalRHubResult WatchStream(long userId);
    SignalRHubResult UnwatchStream(long userId);
}