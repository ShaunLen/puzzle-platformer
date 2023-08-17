using System.Collections.Generic;
using Godot;

namespace PuzzlePlatformer.ui.guide;

public partial class ObjectBtn : Button
{
    public string ObjectName;
    public string ObjectDesc;
    public Dictionary<string, string> Properties;
    public Dictionary<string, string> Methods;
}