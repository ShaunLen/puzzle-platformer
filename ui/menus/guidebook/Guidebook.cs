using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.ui.menus.guidebook;

/* GUIDEBOOK LAYOUT
 * - Level
 *    - Description
 *    - Concepts introduced
 *    - Hints
 *    - Interactables
 *        - Each object
 *        - Properties
 *        - Methods
 * - LiteScript
*/
public partial class Guidebook : Control
{
    /* Signals */
    [Signal] public delegate void GuidebookOpenedEventHandler();
    
    public bool GuidebookOpen;
    
    private TabContainer _tabContainer;
    
    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("TabContainer");

        _tabContainer.TabChanged += tab =>
        {
            AudioManager.Instance.PlaySoundFromList(new List<AudioManager.Sound>
            {
                AudioManager.Sound.PageFlip1, AudioManager.Sound.PageFlip2, AudioManager.Sound.PageFlip3
            });
        };

        CodeManager.Instance.CodeWindowOpened += CloseGuidebook;
    }

    public override void _Process(double delta)
    {
        if(InputManager.IsActionJustPressed(InputManager.Action.ToggleGuidebook, true))
        {
            if(GuidebookOpen)
                CloseGuidebook();
            else
                OpenGuidebook();
        }
    }

    public void OpenGuidebook()
    {
        EmitSignal(SignalName.GuidebookOpened);
        
        GuidebookOpen = true;
        InputManager.InputEnabled = false;
        Input.MouseMode = Input.MouseModeEnum.Confined;

        var tween = CreateTween();
        tween.TweenProperty(_tabContainer, "position:y", 45, 0.1);
    }

    public void CloseGuidebook()
    {
        GuidebookOpen = false;
        InputManager.InputEnabled = true;
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        
        var tween = CreateTween();
        tween.TweenProperty(_tabContainer, "position:y", 545, 0.1);
    }
}