using Godot;

namespace PuzzlePlatformer.addons.camera_2d_plus;

[GlobalClass]
public partial class Camera2DPlus : Camera2D
{
    [ExportGroup("Target Zoom")] 
    [Export] private bool _zoomSmoothingEnabled = true;
    [Export] private int _zoomSmoothingSpeed = 10;

    [ExportGroup("Target Offset")] 
    [Export] private bool _offsetSmoothingEnabled = true;
    [Export] private int _offsetSmoothingSpeed = 10;
    
    /* Targets */
    public Vector2 TargetOffset;
    public Vector2 TargetZoom;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Offset != TargetOffset)
        {
            if (_offsetSmoothingEnabled)
                SmoothOffset(delta);
            else
                Offset = TargetOffset;
        }
        
        if (Zoom != TargetZoom)
        {
            if (_zoomSmoothingEnabled)
                SmoothZoom(delta);
            else
                Zoom = TargetZoom;
        }
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
}