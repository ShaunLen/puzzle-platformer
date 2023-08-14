using System;
using Godot;

namespace PuzzlePlatformer.autoloads;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    public enum Sound
    {
        Error,
        Door,
        DoorStop,
        ButtonPress,
        ButtonRelease
    }

    private AudioStreamPlayer _audioStreamPlayer;

    public override void _Ready()
    {
        Instance = this;
        _audioStreamPlayer = GetTree().GetFirstNodeInGroup("AudioPlayer") as AudioStreamPlayer;
    }

    public void PlaySound(Sound sound, AudioStreamPlayer2D player = null)
    {
        if (player != null)
        {
            player.Stream = sound.ToAudioStream();
            player.Play();
        }
        else
        {
            _audioStreamPlayer.Stream = sound.ToAudioStream();
            _audioStreamPlayer.Play();
        }
    }
}

internal static class AudioExtensions
{
    public static AudioStream ToAudioStream(this AudioManager.Sound sound)
    {
        return sound switch
        {
            AudioManager.Sound.Error => ResourceLoader.Load("res://audio/effects/error.wav") as AudioStream,
            AudioManager.Sound.Door => ResourceLoader.Load("res://audio/effects/door.wav") as AudioStream,
            AudioManager.Sound.DoorStop => ResourceLoader.Load("res://audio/effects/door_stop.wav") as AudioStream,
            AudioManager.Sound.ButtonPress => ResourceLoader.Load("res://audio/effects/button_press.wav") as AudioStream,
            AudioManager.Sound.ButtonRelease => ResourceLoader.Load("res://audio/effects/button_release.wav") as AudioStream,
            _ => null
        };
    }
}