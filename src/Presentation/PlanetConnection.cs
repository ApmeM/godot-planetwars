using System;
using Godot;
using GodotAnalysers;

[SceneReference("PlanetConnection.tscn")]
public partial class PlanetConnection : ISelectable
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

    public int DronesToSend { get; set; } = 5;

    public bool Active { get; set; }

    public int PlayerId { get; set; }

    private bool selected = false;
    private bool selectedDirty = false;
    [Export]
    public bool Selected
    {
        get => selected;
        set
        {
            this.selected = value;
            this.selectedDirty = true;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.AddToGroup(Groups.Selectable);
        this.AddToGroup(Groups.PlanetConnection);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (selectedDirty)
        {
            this.planetConnectionSelectedSprite.Visible = this.selected;
            this.planetConnectionSelectedSprite.Position = (From.Position + To.Position) / 2 + Vector2.Up * 20;
        }

        if (pointsDirty)
        {
            this.Points = new Vector2[]
            {
                From.Position,
                To.Position
            };
            pointsDirty = false;
        }

        if (From.PlayerId != this.PlayerId)
        {
            this.QueueFree();
            return;
        }

        if (Active)
        {
            if (From.DronesCount > DronesToSend)
            {
                var drones = this.DronesScene.Instance<Drones>();
                this.GetParent().AddChild(drones);
                drones.Go(From, To, From.DronesCount);
            }
        }
    }

    public bool IsClicked(Vector2 position)
    {
        if (From == null || To == null)
        {
            return false;
        }

        // Answer is taken from https://www.reddit.com/r/godot/comments/f3dr76/how_to_make_clickable_line2d/
        var pos = ToLocal(position);
        var squared_width = Width * Width;

        for (var i = 0; i < Points.Length - 1; i++)
        {
            var closest_point = Geometry.GetClosestPointToSegment2d(pos, this.Points[i], this.Points[i + 1]);
            if (closest_point.DistanceSquaredTo(pos) <= squared_width)
            {
                return true;
            }
        }
        return false;
    }
}
