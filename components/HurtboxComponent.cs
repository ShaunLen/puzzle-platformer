using Godot;

namespace PuzzlePlatformer.components;

[GlobalClass]
public partial class HurtboxComponent : HitboxComponent
{
    [Export] private HealthComponent _healthComponent;
}