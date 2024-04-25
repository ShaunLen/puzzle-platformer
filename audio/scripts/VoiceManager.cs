using System;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.ui.hud;
using static PuzzlePlatformer.audio.scripts.VoiceManager;

namespace PuzzlePlatformer.audio.scripts;

public partial class VoiceManager : Node
{
	[Signal] public delegate void PlayVoiceLineEventHandler(VoiceLine voiceLine);
	public static VoiceManager Instance { get; private set; }

	public enum VoiceLineType
	{
		Idle,
		LevelRestart,
	}

	public enum VoiceLine
	{
		/* Level Restart Lines */
		LevelRestartLinesStart,
		LevelRestart1,
		LevelRestart2,
		LevelRestartLinesEnd
	}

	public override void _Ready()
	{
		CallDeferred(nameof(CheckForRestarts));
		Instance = this;
	}

	private void CheckForRestarts()
	{
		if(GameManager.Instance.LevelRestarts > 0 && !GameManager.Instance.InIntro && GD.RandRange(0, 100) < 40 && GameManager.Instance.CurrentScene >= GameManager.Scene.LevelOne)
			PlayRandomVoiceLine(VoiceLineType.LevelRestart);
	}

	public void PlayRandomVoiceLine(VoiceLineType voiceLineType)
	{
		// var randomVoiceLine = voiceLineType.GetRandomLine();
		// EmitSignal(SignalName.PlayVoiceLine, (int) randomVoiceLine);
		// HudManager.Instance.AddSubtitle(randomVoiceLine.GetSubtitle(), randomVoiceLine.ToAudioStream().GetLength());
	}
}

internal static class VoiceExtensions
{
	public static AudioStream ToAudioStream(this VoiceLine voiceLine)
	{
		return voiceLine switch
		{
			VoiceLine.LevelRestart1 => ResourceLoader.Load("res://audio/voice/level_restart/level_restart_line_1.wav") as AudioStream,
			VoiceLine.LevelRestart2 => ResourceLoader.Load("res://audio/voice/level_restart/level_restart_line_2.wav") as AudioStream,
			_ => null
		};
	}

	public static VoiceLine GetRandomLine(this VoiceLineType voiceLineType)
	{
		switch (voiceLineType)
		{
			case VoiceLineType.LevelRestart:
				return (VoiceLine)new Random().Next((int) VoiceLine.LevelRestartLinesStart + 1, (int) VoiceLine.LevelRestartLinesEnd);
			default:
				GD.Print("Returned default voice line.");
				return VoiceLine.LevelRestart1;
		}
	}

	public static string GetSubtitle(this VoiceLine voiceLine)
	{
		return voiceLine switch
		{
			VoiceLine.LevelRestart1 => "Keep going. Your feeble attempts are almost amusing.",
			VoiceLine.LevelRestart2 => "Trying again? How adorable.",
			_ => ""
		};
	}
}