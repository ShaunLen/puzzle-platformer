using Godot;
using PuzzlePlatformer.components;

namespace PuzzlePlatformer.ui.key_hint;

public partial class KeyHint : Button
{
	[Export] private FloatComponent _floatComponent;
	private bool _float;
	private bool _showKey;
	public Texture2D KeyIcon;

	public bool Float
	{
		get => _float;
		set
		{
			_float = value;
			_floatComponent.ProcessMode = _float ? ProcessModeEnum.Disabled : ProcessModeEnum.Pausable;
		}
	}

	public bool ShowKey
	{
		get => _showKey;
		set
		{
			_showKey = value;
			Icon = _showKey ? KeyIcon : null;
		}
	}
}
