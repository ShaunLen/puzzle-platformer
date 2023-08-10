using Godot;
using PuzzlePlatformer.addons.camera_2d_plus;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;
using PuzzlePlatformer.litescript.Frontend;
using PuzzlePlatformer.litescript.Runtime;
using PuzzlePlatformer.ui.themes;
using HitboxComponent = PuzzlePlatformer.components.HitboxComponent;

namespace PuzzlePlatformer.objects.interactable.terminal;

public partial class Terminal : Node2D
{
    /* Exports */
    [Export] private Control _terminalScreen;
    
    /* Nodes */
    private Sprite2D _sprite;
    private PointLight2D _light;
    private PointLight2D _closeButtonLight;
    private RemoteTransform2D _playerTransform;
    private Camera2DPlus _camera;
    private CodeEdit _codeEdit;
    private TextureButton _closeButton;
    private HitboxComponent _hitboxComponent;
    private RichTextLabel _console;
    
    /* Palette */
    private NordColors _nordColors;
    
    /* Booleans */
    private bool _closeButtonPressed;
    private bool _playerNearby;
    
    /* Positions */
    private Vector2 _startingOffset;
    private Vector2 _startingZoom;
    private Vector2 _targetOffset = new Vector2(0, 0);
    private Vector2 _targetZoom = new Vector2(2, 2);
    
    /* LiteScript */
    private Parser _parser;
    private Interpreter _interpreter;
    private Env _env;
    
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _light = GetNode<PointLight2D>("LightOrange");
        _hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
        _playerTransform = GetTree().GetFirstNodeInGroup("Player").GetNode<RemoteTransform2D>("PlayerTransform");
        _camera = (Camera2DPlus) GetTree().GetFirstNodeInGroup("Camera");
        _codeEdit = _terminalScreen.GetNode<CodeEdit>("CodeEdit");
        _closeButton = _terminalScreen.GetNode<TextureButton>("CloseButton");
        _closeButtonLight = _terminalScreen.GetNode<PointLight2D>("CloseButtonLight");
        _console = _terminalScreen.GetNode<RichTextLabel>("Console");
        _nordColors = new NordColors();

        _startingOffset = _camera.Offset;
        _startingZoom = _camera.Zoom;

        _parser = new Parser();
        _interpreter = new Interpreter();

        /* Signal Connections */
        _hitboxComponent.PlayerEntered += () => _playerNearby = true;
        _hitboxComponent.PlayerExited += () => _playerNearby = false;
        _closeButton.ButtonDown += () => _closeButtonPressed = true;
        _closeButton.MouseEntered += () => _closeButtonLight.Hide();
        _closeButton.MouseExited += () => _closeButtonLight.Show();

        SetSyntaxHighlighting(_codeEdit.SyntaxHighlighter);
    }

    public override void _PhysicsProcess(double delta)
    {
        _light.Enabled = _sprite.Frame > 3;

        if (_playerNearby && !_codeEdit.HasFocus())
            GetNode<Label>("InteractHint/Text").Show();
        else
            GetNode<Label>("InteractHint/Text").Hide();

        if (InputManager.IsActionJustPressed(InputManager.Action.Interact) && _playerNearby)
            AttachToTerminal(delta);

        if (_closeButtonPressed)
            DetachFromTerminal(delta);
        
        if(InputManager.IsActionJustPressed(InputManager.Action.RunCode, true))
            ExecuteCode();

        if (InputManager.IsActionJustPressed(InputManager.Action.ClearConsole, true))
            ConsoleClear();
        
        if (InputManager.IsActionJustPressed(InputManager.Action.CloseTerminal, true))
            DetachFromTerminal(delta);
    }

    private void ExecuteCode()
    {
        _closeButtonPressed = false;
        
        _env = Env.CreateGlobalEnvironment();
        _env.Terminal = this;
        
        ConsoleClear();
        
        var input = _terminalScreen.GetNode<CodeEdit>("CodeEdit").Text;
        
        var script = _parser.ProduceAst(input);
        var result = _interpreter.Evaluate(script, _env);
        
        ConsoleWriteLine("\n" + "Program finished with 0 errors.");
    }

    public void ConsoleWrite(string output)
    {
        _console.Text += output;
    }

    public void ConsoleWriteLine(string output)
    {
        _console.Text = _console.Text + "\n" + output;
    }

    public void ConsoleClear()
    {
        _console.Text = "";
    }

    private void AttachToTerminal(double delta)
    {
        InputManager.InputEnabled = false;
        GameManager.Instance.TerminalOpen = true;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        _codeEdit.MouseFilter = Control.MouseFilterEnum.Stop;
        Input.WarpMouse(GetTree().Root.Size / 2);
        
        _codeEdit.GrabFocus();
        
        _playerTransform.RemotePath = "";
        _camera.Transform = new Transform2D(_terminalScreen.Rotation, _terminalScreen.GetRect().GetCenter());
        _camera.TargetOffset = _targetOffset;
        _camera.TargetZoom = _targetZoom;
    }

    private void DetachFromTerminal(double delta)
    {
        InputManager.InputEnabled = true;
        GameManager.Instance.TerminalOpen = false;
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        _codeEdit.MouseFilter = Control.MouseFilterEnum.Ignore;
        _closeButtonPressed = false;
        
        _codeEdit.ReleaseFocus();
        
        _playerTransform.RemotePath = _camera.GetPath();
        _camera.TargetOffset = _startingOffset;
        _camera.TargetZoom = _startingZoom;
    }

    private void SetSyntaxHighlighting(SyntaxHighlighter syntaxHighlighter)
    {
        var highlighter = (CodeHighlighter) syntaxHighlighter;
        
        /* General Colors */
        highlighter.NumberColor = _nordColors.Nord15;
        highlighter.SymbolColor = _nordColors.Nord10;
        highlighter.FunctionColor = _nordColors.Nord8;
        highlighter.MemberVariableColor = _nordColors.Nord7;
        
        /* Keywords */
        highlighter.AddKeywordColor("var", _nordColors.Nord7);
        highlighter.AddKeywordColor("const", _nordColors.Nord7);
        highlighter.AddKeywordColor("else", _nordColors.Nord7);

        /* Color Regions */
        highlighter.AddColorRegion("#", "", _nordColors.Nord3);
        highlighter.AddColorRegion("\"", "\"", _nordColors.Nord14);
    }
}