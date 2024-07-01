namespace Colir.Interfaces.Hubs;

public interface IVoiceChatHub
{
    Task Connect(string roomGuid);
    Task Join();
    Task Leave();

    Task MuteSelf();
    Task UnmuteSelf();
    Task DefeanSelf();
    Task UndefeanSelf();
    Task SendVoiceSignal(string audioData);

    Task EnableVideo();
    Task DisableVideo();
    Task SendVideo(string videoData);

    Task EnableStream();
    Task DisableStream();
    Task SendStreamPicture(string pictureData);
}