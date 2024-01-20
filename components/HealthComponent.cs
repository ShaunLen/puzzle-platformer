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
        // GetParent().Ready += CheckForInit; TODO: Why is this not initialised when switching back to a previously visited scene?
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
    
    /* Private Methods */
    private void CheckForInit()
    {
        GetParent().Ready -= CheckForInit;
        if (_initialised)
            return;
                
        GD.PrintErr("HealthComponent was not initialised.");
        GetTree().Quit();
    }
}