using Godot;
using Godot.Collections;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.ui.themes;

namespace PuzzlePlatformer.objects.interactable.terminal;

public partial class Terminal : Node2D
{
    /* Exports */
    [Export] private int _offsetSmoothSpeed = 20;
    [Export] private int _zoomSpeed = 2;
    [Export] private Control _terminalScreen;
    
    /* Nodes */
    private Sprite2D _sprite;
    private PointLight2D _light;
    private RemoteTransform2D _playerTransform;
    private Camera2D _camera;
    private CodeEdit _codeEdit;
    private HitboxComponent _hitboxComponent;
    
    /* Palette */
    private NordColors _nordColors;
    
    /* Booleans */
    private bool _isPlayerAttached;
    private bool _playerNearby;
    
    /* Positions */
    private Vector2 _startingOffset;
    private Vector2 _startingZoom;
    private Vector2 _targetOffset = new Vector2(0, 0);
    private Vector2 _targetZoom = new Vector2(2, 2);
    
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _light = GetNode<PointLight2D>("LightOrange");
        _hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
        _playerTransform = GetTree().GetFirstNodeInGroup("Player").GetNode<RemoteTransform2D>("PlayerTransform");
        _camera = (Camera2D) GetTree().GetFirstNodeInGroup("Camera");
        _codeEdit = _terminalScreen.GetNode<CodeEdit>("CodeEdit");
        _nordColors = new NordColors();

        _startingOffset = _camera.Offset;
        _startingZoom = _camera.Zoom;

        /* Signal Connections */
        _hitboxComponent.PlayerEntered += () => _playerNearby = true;
        _hitboxComponent.PlayerExited += () => _playerNearby = false;

        SetSyntaxHighlighting(_codeEdit.SyntaxHighlighter);
    }

    public override void _PhysicsProcess(double delta)
    {
        _light.Enabled = _sprite.Frame > 3;

        if (InputManager.IsActionJustPressed(InputManager.Action.Interact) && _playerNearby)
        {
            _isPlayerAttached = true;
        }
            
        if (_camera.Zoom != _targetZoom && _camera.Offset != _targetOffset && _isPlayerAttached)
        {
            AttachCameraToTerminal(delta);
        }
        
        if (InputManager.IsActionJustPressed(InputManager.Action.Close, true) || !_isPlayerAttached)
            AttachCameraToPlayer(delta);
    }

    private void AttachCameraToTerminal(double delta)
    {
        if (!_isPlayerAttached)
        {
            _isPlayerAttached = true;
            _playerTransform.RemotePath = "";
            _camera.Transform = new Transform2D(_terminalScreen.Rotation, _terminalScreen.GetRect().GetCenter());
            _codeEdit.GrabFocus();
            InputManager.SetInputEnabled(false);
        }

        if (_camera.Zoom == _targetZoom || _camera.Offset == _targetOffset || !_isPlayerAttached)
            return;

        _camera.Offset = new Vector2(_startingOffset.X, Mathf.MoveToward(_camera.Offset.Y, _targetOffset.Y, (float) delta * 10 * _offsetSmoothSpeed));
        _camera.Zoom = new Vector2(Mathf.MoveToward(_camera.Zoom.X, _targetZoom.X, (float) delta * _zoomSpeed),
            Mathf.MoveToward(_camera.Zoom.Y, _targetZoom.Y, (float) delta * _zoomSpeed));
    }

    private void AttachCameraToPlayer(double delta)
    {
        _playerTransform.RemotePath = _camera.GetPath();
        _camera.Offset = new Vector2(0, Mathf.MoveToward(_camera.Offset.Y, -100, (float) delta * 10 * _offsetSmoothSpeed));
        _camera.Zoom = new Vector2(Mathf.MoveToward(_camera.Zoom.X, 1, (float) delta * _zoomSpeed), Mathf.MoveToward(_camera.Zoom.Y, 1, (float) delta * _zoomSpeed));
        _codeEdit.ReleaseFocus();
        InputManager.SetInputEnabled(true);
        _isPlayerAttached = false;
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
        highlighter.AddKeywordColor("int", _nordColors.Nord7);
        highlighter.AddKeywordColor("double", _nordColors.Nord7);
        highlighter.AddKeywordColor("float", _nordColors.Nord7);
        highlighter.AddKeywordColor("bool", _nordColors.Nord7);
        highlighter.AddKeywordColor("string", _nordColors.Nord7);

        /* Color Regions */
        highlighter.AddColorRegion("#", "", _nordColors.Nord3);
        highlighter.AddColorRegion("\"", "\"", _nordColors.Nord14);
        highlighter.AddColorRegion("'", "'", _nordColors.Nord14);
    }
}