using System;
using Godot;
using GodotAnalysers;

[SceneReference("PlanetConnection.tscn")]
public partial class PlanetConnection
{
    private Planet from;
    private Planet to;
    private bool pointsDirty = true;
    
    [Export]
    public PackedScene DronesScene;

    public Planet From
    {
        get => from;
        set
        {
            pointsDirty = true;
            from = value;
        }
    }

    public Planet To
    {
        get => to;
        set
        {
            pointsDirty = true;
            to = value;
        }
    }

    public bool Active { get; set; }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (pointsDirty)
        {
            this.Points = new Vector2[]
            {
                From?.Position ?? Vector2.Zero,
                To?.Position ?? Vector2.Zero
            };
            pointsDirty = false;
        }

        if (To == null)
        {
            this.Points = new Vector2[]
            {
                From?.Position ?? Vector2.Zero,
                this.GetGlobalMousePosition()
            };
        }

        if (Active)
        {
            if (From.DronesCount > 0)
            {
                var drones = this.DronesScene.Instance<Drones>();
                this.GetParent<Game>().AddChild(drones);
                drones.Go(From, To);
            }
        }
    }
}
