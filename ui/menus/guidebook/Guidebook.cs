using Godot;

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
    private Tree _levelTree;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        _levelTree = GetNode<Tree>("TabContainer/LevelTree");
        
        var descriptionHeader = _levelTree.CreateItem();
        descriptionHeader.SetText(0, "Description");

        var description = _levelTree.CreateItem(descriptionHeader);
        description.SetText(0, "This is a simple test description for a level. This level contains two objects - " +
                               "\nRedButton and BlueDoor. Try interacting with them via the PocketTerminal!");
    }
}