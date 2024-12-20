﻿using Colir.Communication.Enums;
using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IVoiceChatHub
{
    /// <summary>
    /// Gets a list of users connected to the voice channel
    /// </summary>
    SignalRHubResult GetVoiceChatUsers();

    /// <summary>
    /// Joins the user to the voice channel. Notifies others with the "UserJoined" signal
    /// </summary>
    Task<SignalRHubResult> Join(bool isMuted, bool isDeafened);

    /// <summary>
    /// Disconnects the user from the voice channel. Notifies others with the "UserLeft" signal
    /// </summary>
    Task<SignalRHubResult> Leave();

    /// <summary>
    /// Mutes the user's microphone. Notifies others with the "UserMuted" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> MuteSelf();

    /// <summary>
    /// Unmutes the user's microphone. Notifies others with the "UserUnmuted" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> UnmuteSelf();

    /// <summary>
    /// Deafens the user's headphones. Notifies others with the "UserDeafened" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> DeafenSelf();

    /// <summary>
    /// Undeafens the user's headphones. Notifies others with the "UserUndeafened" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> UndeafenSelf();

    /// <summary>
    /// Sends a voice signal to users who are connected to the voice channel (except those who are currently deafened) with the "ReceiveVoiceSignal" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    /// <param name="audioData">The audio data to send</param>
    Task<SignalRHubResult> SendVoiceSignal(string audioData);

    /// <summary>
    /// Enables the user's camera. Notifies others with the "UserEnabledVideo" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> EnableVideo();

    /// <summary>
    /// Disables the user's camera. Notifies others with the "UserDisabledVideo" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> DisableVideo();

    /// <summary>
    /// Sends a video signal to users who are connected to the voice channel with the "ReceiveVideoSignal" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    /// <param name="videoData">The video data to send</param>
    Task<SignalRHubResult> SendVideoSignal(string videoData);

    /// <summary>
    /// Enables a screen share. Notifies others with the "UserEnabledStream" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> EnableStream();

    /// <summary>
    /// Disables the screen share. Notifies others with the "UserDisabledStream" signal
    /// An error with <see cref="ErrorCode.YouAreNotConnectedToVoiceChannel"/> code is returned when the user is not connected to the voice channel yet
    /// </summary>
    Task<SignalRHubResult> DisableStream();

    /// <summary>
    /// Sends a screen share signal to users who are connected to the voice channel (those who enabled streaming from this user) with the "ReceiveStreamData" signal
    /// </summary>
    /// <param name="pictureData">The data to send</param>
    Task<SignalRHubResult> SendStreamSignal(string pictureData);

    /// <summary>
    /// Enables the user to receive picture data of the screen share from a certain user
    /// </summary>
    /// <param name="userHexId">Hex Id of the user to receive screen share data from</param>
    SignalRHubResult WatchStream(long userHexId);

    /// <summary>
    /// Enables the user to stop receiving picture data of the screen share from a certain user
    /// </summary>
    /// <param name="userHexId">Hex Id of the user to stop receiving screen share data from</param>
    SignalRHubResult UnwatchStream(long userHexId);
}