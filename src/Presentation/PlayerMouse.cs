using Godot;
using GodotAnalysers;
using System;
using System.Linq;

[SceneReference("PlayerMouse.tscn")]
public partial class PlayerMouse
{
    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    [Export]
    public int PlayerId { get; set; }

    private Planet From;
    private Planet To;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._Input(@event);
        if (!(@event is InputEventMouseButton mouse))
        {
            return;
        }

        if (mouse.ButtonIndex == (int)ButtonList.Left)
        {
            if (mouse.Doubleclick)
            {
                var connection = this.GetTree().GetNodesInGroup(Groups.Selectable)
                    .OfType<PlanetConnection>()
                    .Where(a => a.PlayerId == this.PlayerId)
                    .Where(a => a.IsClicked(mouse.Position))
                    .FirstOrDefault();
                if (connection != null)
                {
                    connection.QueueFree();
                }
            }
            else if (mouse.Pressed)
            {
                var draggingFrom = this.GetTree().GetNodesInGroup(Groups.Selectable)
                    .Cast<ISelectable>()
                    .Where(a => a.PlayerId == this.PlayerId)
                    .Where(a => a.IsClicked(mouse.Position))
                    .FirstOrDefault();

                if (draggingFrom != null && draggingFrom is Planet planet)
                {
                    this.From = planet;
                    this.Visible = true;
                }
            }
            else if (From != null)
            {
                this.GetParent<Game>().MakeConnection(this.PlayerId, this.From, this.To);
                From = null;
                this.Visible = false;
            }
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!this.Visible)
        {
            return;
        }

        var mousePosition = this.GetGlobalMousePosition();
        this.To = this.GetTree().GetNodesInGroup(Groups.Planet)
            .Cast<Planet>()
            .Where(a => a.IsClicked(mousePosition))
            .FirstOrDefault();

        this.Points = new Vector2[]
        {
            From.Position,
            To?.Position ?? this.GetGlobalMousePosition()
        };
    }
}
