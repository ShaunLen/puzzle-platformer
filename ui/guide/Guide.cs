using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.objects.interactable;
using PuzzlePlatformer.world.levels;

namespace PuzzlePlatformer.ui.guide;

public partial class Guide : Control
{
	/* Nodes */
	private LevelRoot _level;
	private RichTextLabel _levelDesc;
	private RichTextLabel _objectDesc;
	private Label _rightPaneLabel;
	private VBoxContainer _requirementsContainer;
	private VBoxContainer _goalsContainer;
	private VBoxContainer _propertiesContainer;
	private VBoxContainer _methodsContainer;
	private VBoxContainer _objectsContainer;
	[Export] private VBoxContainer _nativeFuncsContainer;
	[Export] private Button _nativeFuncsButton;
	[Export] private Button _closeButton;
	private TabContainer _rightPane;
	private Button _overviewButton;
	
	/* Packed Scenes */
	private PackedScene _requirement;
	private PackedScene _objectBtn;
	private PackedScene _property;

	public override void _Ready()
	{
		Show();
		
		_level = (LevelRoot) GetTree().CurrentScene;

		_requirement = GD.Load<PackedScene>("res://ui/guide/requirement.tscn");
		_objectBtn = GD.Load<PackedScene>("res://ui/guide/object_btn.tscn");
		_property = GD.Load<PackedScene>("res://ui/guide/property.tscn");
		
		//TODO: Move these to exports instead of hard coded paths.
		
		_levelDesc = GetNode<RichTextLabel>("NinePatchRect/RightPane/TabContainer/LevelOverview/LevelOverviewContainer/MarginContainer/LevelDesc");
		_objectDesc = GetNode<RichTextLabel>("NinePatchRect/RightPane/TabContainer/ObjectOverview/ObjectOverviewContainer/MarginContainer/ObjectDesc");
		_rightPaneLabel = GetNode<Label>("NinePatchRect/RightPane/RightPaneHeader/RightPaneHeaderLabel");
		_requirementsContainer = GetNode<VBoxContainer>("NinePatchRect/RightPane/TabContainer/LevelOverview/LevelOverviewContainer/RequirementsContainer");
		_goalsContainer = GetNode<VBoxContainer>("NinePatchRect/RightPane/TabContainer/LevelOverview/LevelOverviewContainer/GoalsContainer");
		_propertiesContainer = GetNode<VBoxContainer>("NinePatchRect/RightPane/TabContainer/ObjectOverview/ObjectOverviewContainer/PropertiesContainer");
		_methodsContainer = GetNode<VBoxContainer>("NinePatchRect/RightPane/TabContainer/ObjectOverview/ObjectOverviewContainer/MethodsContainer");
		_objectsContainer = GetNode<VBoxContainer>("NinePatchRect/LeftPane/ObjectsPanel/ObjectsScrollContainer/ObjectsContainer");
		_rightPane = GetNode<TabContainer>("NinePatchRect/RightPane/TabContainer");
		_overviewButton = GetNode<Button>("NinePatchRect/LeftPane/LinksPanel/LinksContainer/LevelOverviewBtn");
		
		/* Signal Connections */
		
		_level.Ready += () =>
		{
			PopulateLevelOverview();
			
			foreach (var node in _objectsContainer.GetChildren())
			{
				var objBtn = (ObjectBtn) node;
				objBtn.Pressed += () => PopulateObjectOverview(objBtn);
			}
		};

		_overviewButton.Pressed += () =>
		{
			_rightPaneLabel.Text = _level.LevelName;
			_rightPane.CurrentTab = 0;
		};

		_nativeFuncsButton.Pressed += PopulateNativeFunctions;
		_closeButton.Pressed += CloseGuide;

		UiManager.Instance.OpenGuide += OpenGuide;
		UiManager.Instance.CloseGuide += CloseGuide;
	}

	private void OpenGuide()
	{
		UiManager.Instance.GuideOpen = true;
		
		InputManager.InputEnabled = false;
		Input.MouseMode = Input.MouseModeEnum.Confined;
		Input.WarpMouse(GetTree().Root.Size / 2);

		var tween = CreateTween();
		tween.TweenProperty(this, "position:y", 0, 0.1);
	}

	private void CloseGuide()
	{
		UiManager.Instance.GuideOpen = false;
		
		InputManager.InputEnabled = true;
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;

		var tween = CreateTween();
		tween.TweenProperty(this, "position:y", 495, 0.1);
	}

	private void PopulateLevelOverview()
	{
		_rightPaneLabel.Text = _level.LevelName;
		_levelDesc.Text = "[center]" + _level.LevelDesc;

		foreach (var req in GetNode<LevelRoot>(GetTree().CurrentScene.GetPath()).Requirements)
		{
			var instance = _requirement.Instantiate();
			instance.GetNode<RichTextLabel>("RequirementDesc").Text = "[center]" + req.Desc;
			
			if(req.Required)
				_requirementsContainer.AddChild(instance);
			else
				_goalsContainer.AddChild(instance);
		}

		foreach (var node in GetTree().GetNodesInGroup("Interactable"))
		{
			var obj = (Interactable) node;
			var instance = _objectBtn.Instantiate() as ObjectBtn;
			
			instance!.Text = obj.Name;
			instance.ObjectName = obj.Name;
			instance.ObjectDesc = obj.Description;
			instance.Properties = obj.Properties;
			instance.Methods = obj.Methods;
				
			_objectsContainer.AddChild(instance);
		}
	}

	private void PopulateObjectOverview(ObjectBtn btn)
	{
		_rightPane.CurrentTab = 1;
		_rightPaneLabel.Text = btn.ObjectName;
		_objectDesc.Text = "[center]" + btn.ObjectDesc;

		// Remove previous properties
		foreach (var node in _propertiesContainer.GetChildren())
		{
			if(node.GetType() != typeof(NinePatchRect))
				node.QueueFree();
		}
		
		// Remove previous methods
		foreach (var node in _methodsContainer.GetChildren())
		{
			if(node.GetType() != typeof(NinePatchRect))
				node.QueueFree();
		}

		// Add object properties
		foreach (var property in btn.Properties)
		{
			var instance = _property.Instantiate() as GuideProperty;

			instance!.SetName(property.Key);
			instance.SetDesc(property.Value);
			
			_propertiesContainer.AddChild(instance);
		}
		
		// Add object methods
		foreach (var property in btn.Methods)
		{
			var instance = _property.Instantiate() as GuideProperty;

			instance!.SetName(property.Key);
			instance.SetDesc(property.Value);
			
			_methodsContainer.AddChild(instance);
		}
	}

	private void PopulateNativeFunctions()
	{
		_rightPane.CurrentTab = 2;
		_rightPaneLabel.Text = "Native Functions";
		
		foreach (var node in _nativeFuncsContainer.GetChildren())
			node.QueueFree();
		
		var printNative = _property.Instantiate() as GuideProperty;
		printNative!.SetName("Print()");
		printNative.SetDesc("Logs a message to the console.\nArgs: string message");
		_nativeFuncsContainer.AddChild(printNative);
	}
}
