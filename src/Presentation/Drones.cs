using Godot;
using GodotAnalysers;

[SceneReference("Drones.tscn")]
[Tool]
public partial class Drones
{
    private int dronesCount = 0;
    private bool dronesCountDirty = true;

    [Export]
    public int DronesCount
    {
        get => dronesCount;
        set
        {
            this.dronesCount = value;
            this.dronesCountDirty = true;
        }
    }

    public Planet To { get; set; }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (this.dronesCountDirty)
        {
            this.dronesCountLabel.Text = this.dronesCount.ToString();
        }
    }
}
