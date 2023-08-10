using Godot;

namespace PuzzlePlatformer.components;

[GlobalClass]
public partial class HealthComponent : Node2D
{
    /* Signals */
    [Signal] public delegate void HealthChangedEventHandler(float amount);
    [Signal] public delegate void DiedEventHandler();
    
    /* Variables */
    private bool _initialised;
    
    /* Properties */
    public int MaxHealth { get; set; }
    
    /* Override Methods */
    public override void _Ready()
    {
        GetParent().Ready += () =>
        {
            if (_initialised)
                return;

            const string errorMsg = "HealthComponent was not initialised.";
                
            GD.PrintErr("HealthComponent was not initialised.");
            GetTree().Quit();
        };
    }

    /* Public Methods */
    public void Init(int maxHealth)
    {
        MaxHealth = maxHealth;
        _initialised = true;
    }
    
    public void Damage(float damage)
    {
        EmitSignal(SignalName.HealthChanged, -damage);
    }

    public void Heal(float heal)
    {
        Damage(-heal);
        EmitSignal(SignalName.HealthChanged, heal);
    }

    public void Kill()
    {
        EmitSignal(SignalName.Died);
    }
}