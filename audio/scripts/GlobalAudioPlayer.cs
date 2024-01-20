using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.audio.scripts;

public partial class GlobalAudioPlayer : AudioStreamPlayer
{
	public override void _Ready()
	{
		if(GameManager.Instance.CanRestart)
			PlaySound(AudioManager.Sound.Static);
		
		AudioManager.Instance.PlayGlobalSound += PlaySound;
	}

	public override void _ExitTree()
	{
		AudioManager.Instance.PlayGlobalSound -= PlaySound;
	}

	private void PlaySound(AudioManager.Sound sound)
	{
		Stream = sound.ToAudioStream();
		Play();
	}
}
