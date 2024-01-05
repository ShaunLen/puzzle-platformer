using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.audio.scripts;

public partial class GlobalAudioPlayer : AudioStreamPlayer
{
	public override void _Ready()
	{
		AudioManager.Instance.PlayGlobalSound += PlaySound;
	}

	private void PlaySound(AudioManager.Sound sound)
	{
		Stream = sound.ToAudioStream();
		Play();
	}
}