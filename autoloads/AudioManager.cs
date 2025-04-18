using System;
using System.Collections.Generic;
using Godot;

namespace PuzzlePlatformer.autoloads;

public partial class AudioManager : Node
{
    [Signal] public delegate void PlayGlobalSoundEventHandler(Sound sound);
    public static AudioManager Instance { get; private set; }

    public enum Sound
    {
        Error,
        Door,
        DoorStop,
        ButtonPress,
        ButtonRelease,
        Fabricate,
        Static,
        Shutter,
        Footsteps,
        Jump,
        Sizzle
    }

    public override void _Ready()
    {
        Instance = this;
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
            EmitSignal(SignalName.PlayGlobalSound, (int) sound);
        }
    }
    
    public void PlaySoundFromList(List<Sound> sounds, AudioStreamPlayer2D player = null)
    {
        var random = new Random();
        var sound = sounds[random.Next(sounds.Count)];

        PlaySound(sound, player);
    }

    public void StopSound(AudioStreamPlayer2D player)
    {
        player.Stop();
        // TODO: Handle global audio?
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
            AudioManager.Sound.Fabricate => ResourceLoader.Load("res://audio/effects/warp-sound.wav") as AudioStream,
            AudioManager.Sound.Static => ResourceLoader.Load("res://audio/effects/static.wav") as AudioStream,
            AudioManager.Sound.Shutter => ResourceLoader.Load("res://audio/effects/shutter.wav") as AudioStream,
            AudioManager.Sound.Footsteps => ResourceLoader.Load("res://audio/effects/footsteps.wav") as AudioStream,
            AudioManager.Sound.Jump => ResourceLoader.Load("res://audio/effects/jump.wav") as AudioStream,
            AudioManager.Sound.Sizzle => ResourceLoader.Load("res://audio/effects/sizzle.wav") as AudioStream,
            _ => null
        };
    }
}