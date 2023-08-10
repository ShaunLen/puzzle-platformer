using Godot;
using PuzzlePlatformer.world.levels;

namespace PuzzlePlatformer.addons.camera_2d_plus;

[GlobalClass]
public partial class Camera2DPlus : Camera2D
{
    [ExportGroup("Zoom Smoothing")] 
    [Export] private bool _zoomSmoothingEnabled = true;
    [Export(PropertyHint.Range, "1,5")] private int _zoomSmoothingSpeed = 2;

    [ExportGroup("Offset Smoothing")] 
    [Export] private bool _offsetSmoothingEnabled = true;
    [Export(PropertyHint.Range, "1,10")] private int _offsetSmoothingSpeed = 10;

    [ExportGroup("Limits")]
    [Export] private bool _useLevelBounds = true;
    
    /* Targets */
    public Vector2 TargetOffset;
    public Vector2 TargetZoom;

    private bool _initialised;

    public override void _Ready()
    {
        TargetOffset = Offset;
        TargetZoom = Zoom;
        _offsetSmoothingSpeed *= 20;

        if (_useLevelBounds)
            SetLimits();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (Offset.Y != 0)
            GD.Print(Offset.Y);

        if (!_initialised)
            PositionSmoothingEnabled = false;

        if (Offset != TargetOffset)
        {
            if (_offsetSmoothingEnabled && _initialised)
                SmoothOffset(delta);
            else
                Offset = TargetOffset;
        }
        
        if (Zoom != TargetZoom)
        {
            if (_zoomSmoothingEnabled && _initialised)
                SmoothZoom(delta);
            else
                Zoom = TargetZoom;
        }

        _initialised = true;
        PositionSmoothingEnabled = true;
    }

    private void SmoothOffset(double delta)
    {
        var d = (float) delta * _offsetSmoothingSpeed;
        Offset = new Vector2(Mathf.MoveToward(Offset.X, TargetOffset.X, d), Mathf.MoveToward(Offset.Y, TargetOffset.Y, d));
    }

    private void SmoothZoom(double delta)
    {
        var d = (float) delta * _zoomSmoothingSpeed;
        Zoom = new Vector2(Mathf.MoveToward(Zoom.X, TargetZoom.X, d), Mathf.MoveToward(Zoom.Y, TargetZoom.Y, d));
    }

    private void SetLimits()
    {
        var bounds = GetNode<LevelRoot>("../Level").LevelBounds;

        LimitLeft = 0;
        LimitBottom = 0;
        LimitRight = (int) bounds.X;
        LimitTop = (int) -bounds.Y;
    }
}