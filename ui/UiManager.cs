using System;
using Godot;
using PuzzlePlatformer.autoloads;
using static PuzzlePlatformer.autoloads.AudioManager.Sound;

namespace PuzzlePlatformer.ui;

public partial class UiManager : Node
{
    public static UiManager Instance { get; private set; }
    
    [Signal] public delegate void OpenCodeInterfaceEventHandler();
    [Signal] public delegate void CloseCodeInterfaceEventHandler();
    
    [Signal] public delegate void OpenGuideEventHandler();
    [Signal] public delegate void CloseGuideEventHandler();

    public bool CodeInterfaceOpen;
    public bool GuideOpen;

    public override void _Ready() => Instance = this;

    public override void _Process(double delta)
    {
        if (InputManager.IsActionJustPressed(InputManager.Action.ToggleCode, true))
        {
            if (CodeInterfaceOpen)
                EmitSignal(SignalName.CloseCodeInterface);
            else
            {
                if (GuideOpen)
                    EmitSignal(SignalName.CloseGuide);
                
                EmitSignal(SignalName.OpenCodeInterface);
            }
        }

        if (InputManager.IsActionJustPressed(InputManager.Action.ToggleGuide, true))
        {
            if (GuideOpen)
                EmitSignal(SignalName.CloseGuide);
            else
            {
                if (CodeInterfaceOpen)
                    EmitSignal(SignalName.CloseCodeInterface);
                
                EmitSignal(SignalName.OpenGuide);
            }
        }
    }
}