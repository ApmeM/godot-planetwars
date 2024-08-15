using System;
using System.Linq;
using Godot;
using GodotAnalysers;
using GodotTemplate.Presentation.Utils;

[SceneReference("Game.tscn")]
public partial class Game
{
    [Export]
    public PackedScene PlanetScene;

    [Export]
    public PackedScene DronesScene;

    [Signal]
    public delegate void EndGame(int score);

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.Connect(CommonSignals.VisibilityChanged, this, nameof(VisibilityChanged));
    }

    private void VisibilityChanged()
    {
        this.hUD.Visible = this.Visible;
    }

    private bool isDragging = false;
    private Planet draggingFrom = null;

    public Node2D GameField => this.gameField;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!(@event is InputEventMouseButton mouse))
        {
            return;
        }

        if (mouse.ButtonIndex == (int)ButtonList.Left)
        {
            if (mouse.Pressed)
            {
                draggingFrom = this.GetTree().GetNodesInGroup(Groups.Planet)
                    .Cast<Planet>()
                    .Where(a => a.PlayerId == Constants.PlayerAllyId)
                    .Where(a => a.GetRect().HasPoint(a.ToLocal(mouse.Position)))
                    .FirstOrDefault();
                this.mouseDirectionLine.Points = new Vector2[]
                {
                    draggingFrom?.Position ?? Vector2.Zero
                };
            }
            else if (draggingFrom != null)
            {
                if (draggingFrom.GetRect().HasPoint(draggingFrom.ToLocal(mouse.Position)))
                {
                    if (this.planetDetails.Details != null)
                    {
                        this.planetDetails.Details.Selected = false;
                    }
                    this.planetDetails.Visible = true;
                    this.planetDetails.Details = draggingFrom;
                    this.planetDetails.Details.Selected = true;
                }
                else if (draggingFrom.DronesCount > 0)
                {
                    var draggingTo = this.GetTree().GetNodesInGroup(Groups.Planet)
                        .Cast<Planet>()
                        .Where(a => a.GetRect().HasPoint(a.ToLocal(mouse.Position)))
                        .FirstOrDefault();

                    if (draggingTo != null)
                    {
                        var drones = this.DronesScene.Instance<Drones>();
                        drones.DronesCount = draggingFrom.DronesCount;
                        drones.To = draggingTo;
                        drones.Position = draggingFrom.Position;
                        drones.PlayerId = draggingFrom.PlayerId;
                        this.AddChild(drones);

                        draggingFrom.DronesCount = 0;

                        var tween = this.CreateTween();
                        tween.TweenProperty(drones, "position", draggingTo.Position, (draggingFrom.Position - draggingTo.Position).Length() / 50);
                        tween.TweenCallback(this, nameof(DronesArrived), new Godot.Collections.Array { drones });
                    }
                }

                draggingFrom = null;
            }

            isDragging = draggingFrom != null;
            this.mouseDirectionLine.Visible = isDragging;
        }
    }

    private void DronesArrived(Drones drones)
    {
        if (drones.To.PlayerId == drones.PlayerId)
        {
            drones.To.DronesCount += drones.DronesCount;
        }
        else
        {
            if (drones.DronesCount > drones.To.DronesCount)
            {
                drones.To.PlayerId = drones.PlayerId;
            }
            drones.To.DronesCount = Math.Abs(drones.DronesCount - drones.To.DronesCount);
        }

        drones.QueueFree();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!this.Visible)
        {
            return;
        }

        if (isDragging)
        {
            var mousePosition = this.GetGlobalMousePosition();
            var draggingTo = this.GetTree().GetNodesInGroup(Groups.Planet)
                .Cast<Planet>()
                .Where(a => a.GetRect().HasPoint(a.ToLocal(mousePosition)))
                .FirstOrDefault();

            this.mouseDirectionLine.Points = new Vector2[]
            {
                this.mouseDirectionLine.Points[0],
                draggingTo?.Position ?? this.GetGlobalMousePosition()
            };
        }

        if (!this.GetTree().GetNodesInGroup(Groups.Planet)
                .Cast<Planet>()
                .Where(a => a.PlayerId == Constants.PlayerAllyId)
                .Any())
        {
            EmitSignal(nameof(EndGame), 0);
            return;
        }

        if (!this.GetTree().GetNodesInGroup(Groups.Planet)
                .Cast<Planet>()
                .Where(a => a.PlayerId == Constants.PlayerEnemyId)
                .Any())
        {
            EmitSignal(nameof(EndGame), 3);
            this.achievementNotifications.UnlockAchievement("MyFirstAchievement");
            return;
        }
    }
}
