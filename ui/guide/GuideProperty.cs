using Godot;

namespace PuzzlePlatformer.ui.guide;

public partial class GuideProperty : MarginContainer
{
    public void SetName(string name)
    {
        GetNode<Button>("PropertyContainer/PropertyName").Text = name;
    }
    
    public void SetDesc(string desc)
    {
        GetNode<RichTextLabel>("PropertyContainer/PropertyDesc").Text = desc;
    }
}