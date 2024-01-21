using System.Linq;
using System.Text;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.objects.win_button;
using PuzzlePlatformer.world;
using PuzzlePlatformer.world.levels.requirements;

namespace PuzzlePlatformer.ui.report;

public partial class Report : Control
{
	/* External Components */
	[ExportCategory("External Components")]
	[Export] private WinButton _winButton;
	
	/* Internal Components */
	[ExportCategory("Internal Components")] 
	[Export] private Label _reportHeaderLabel;
	[Export] private Label _dividerOne;
	[Export] private Label _trialInformationLabel;
	[Export] private Label _dividerTwo;
	[Export] private VBoxContainer _resultsContainer;
	[Export] private Label _dividerThree;
	[Export] private Label _gradeLabel;
	[Export] private Label _dividerFour;
	[Export] private HBoxContainer _keyHintContainer;
	[Export] private ColorRect _blackBackground;
	[Export] private Button _skipHint;
	
	/* Fields */
	private bool _resultsShown;
	private Tween _tween;
	
	/* Override Methods */
	
	public override void _Ready()
	{
		PopulateTrialInformation();
		_keyHintContainer.Hide();
		_winButton.Pressed += () =>
		{
			ShowReport();
			ProcessMode = ProcessModeEnum.Always;
		};
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;

		if (InputManager.IsActionJustPressed(InputManager.Action.Interact, true) && _resultsShown)
		{
			InputManager.InputEnabled = true;
			GameManager.Instance.ChangeScene(LevelManager.Instance.NextLevel);
		}

		if(InputManager.IsActionJustPressed(InputManager.Action.Jump, true) && !_resultsShown)
			Skip();

		if (InputManager.IsActionJustPressed(InputManager.Action.RestartLevel, true) && _resultsShown)
		{
			InputManager.InputEnabled = true;
			GameManager.Instance.ReloadScene();
		}
	}

	/* Private Methods */
	
	private void PopulateTrialInformation()
	{
		var info = new StringBuilder();
		info.AppendLine("Test Subject: #95742 (SECTOR G2)");
		info.AppendLine($"Trial ID: {LevelManager.Instance.CurrentLevel.LevelName}");
		info.AppendLine($"Date: {Time.GetDateStringFromSystem()}");
		_trialInformationLabel.Text = info.ToString();
	}

	private void PopulateLevelResults()
	{
		var requirements = LevelManager.Instance.CurrentLevel.Requirements;
		foreach (var requirement in requirements)
			AddResult(requirement);
	}

	private void AddResult(Requirement requirement)
	{
		var labelSettings = new LabelSettings();
		labelSettings.Font = GD.Load<FontFile>("res://ui/fonts/DinaRemasterII-Bold-02.ttf");
		labelSettings.FontSize = 14;
		
		var requirementLabel = new Label();
		requirementLabel.LabelSettings = labelSettings;
		requirementLabel.HorizontalAlignment = HorizontalAlignment.Center;
		requirementLabel.LabelSettings.FontColor = requirement.RequirementMet(LevelManager.Instance.Program)
			? Color.FromHtml("#05740b")
			: Color.FromHtml("#9c0a0b");
		
		requirementLabel.Text = requirement.Desc;
		_resultsContainer.AddChild(requirementLabel);
	}

	private void CalculateGrade()
	{
		var scoreEarned = 0d;
		var scoreMax = 0d;
		
		foreach (var requirement in LevelManager.Instance.CurrentLevel.Requirements.Where(requirement => !requirement.Required))
		{
			scoreMax++;

			if (requirement.RequirementMet(LevelManager.Instance.Program))
				scoreEarned++;
		}
		
		var finalScore = scoreEarned / scoreMax;
		_gradeLabel.Text = finalScore switch
		{
			>= 0.99 => "Grade: A",
			>= 0.75 => "Grade: B",
			>= 0.5 => "Grade: C",
			_ => "Grade: D"
		};
	}

	private void ShowReport()
	{
		InputManager.InputEnabled = false;
		
		PopulateLevelResults();
		CalculateGrade();
		HideAllText();
		Show();

		_tween = CreateTween();

		_tween.TweenProperty(_reportHeaderLabel, "visible_characters", _reportHeaderLabel.Text.Length, 1);
		_tween.TweenProperty(_dividerOne, "visible_characters", _dividerOne.Text.Length, 1);
		_tween.TweenProperty(_trialInformationLabel, "visible_characters", _trialInformationLabel.Text.Length, 2);
		_tween.TweenProperty(_dividerTwo, "visible_characters", _dividerTwo.Text.Length, 1);

		foreach (var result in _resultsContainer.GetChildren())
		{
			if (result is Label label)
				_tween.TweenProperty(label, "visible_characters", label.Text.Length, 1);
		}

		_tween.TweenProperty(_dividerThree, "visible_characters", _dividerThree.Text.Length, 1);
		_tween.TweenProperty(_gradeLabel, "visible_characters", _gradeLabel.Text.Length, 2);
		_tween.TweenProperty(_dividerFour, "visible_characters", _dividerFour.Text.Length, 1);
		_tween.TweenProperty(_keyHintContainer, "modulate:a", 255, 2);
			
		_tween.Finished += () =>
		{
			_keyHintContainer.Show();
			_skipHint.QueueFree();
			_resultsShown = true;
		};
	}

	private void HideAllText()
	{
		// Yes of course there is a better way to do this but time constraints > recursion :)
		foreach (var child in GetChildren())
		{	
			if (child is Label label)
				label.VisibleCharacters = 0;

			foreach (var grandchild in child.GetChildren())
			{
				if (grandchild is Label labelTwo)
					labelTwo.VisibleCharacters = 0;

				foreach (var greatGrandchild in grandchild.GetChildren())
				{
					if (greatGrandchild is Label labelThree)
						labelThree.VisibleCharacters = 0;
				}
			}
		}
	}
	
	private void Skip()
	{
		_skipHint.QueueFree();
		_resultsShown = true;
		_tween.Stop();
		_keyHintContainer.Show();
		
		// Yes of course there is a better way to do this but time constraints > recursion :)
		foreach (var child in GetChildren())
		{	
			if (child is Label label)
				label.VisibleCharacters = label.Text.Length;

			foreach (var grandchild in child.GetChildren())
			{
				if (grandchild is Label labelTwo)
					labelTwo.VisibleCharacters = labelTwo.Text.Length;

				foreach (var greatGrandchild in grandchild.GetChildren())
				{
					if (greatGrandchild is Label labelThree)
						labelThree.VisibleCharacters = labelThree.Text.Length;
				}
			}
		}
	}
}
