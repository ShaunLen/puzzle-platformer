using Godot;

namespace PuzzlePlatformer.audio.scripts;

public partial class VoicePlayer : AudioStreamPlayer
{
	public override void _Ready()
	{
		VoiceManager.Instance.PlayVoiceLine += PlayVoiceLine;
	}

	public override void _ExitTree()
	{
		VoiceManager.Instance.PlayVoiceLine -= PlayVoiceLine;
	}

	private void PlayVoiceLine(VoiceManager.VoiceLine voiceLine)
	{
		Stream = voiceLine.ToAudioStream();
		Play();
	}
}