using Godot;
using PuzzlePlatformer.entities.player;

namespace PuzzlePlatformer.autoloads;

public partial class GameLib : Node
{
    public Player GetPlayer()
    {
        return (Player) GetTree().GetFirstNodeInGroup("Player");
    }

    public static void TestTest()
    {
        
    }
}