using System;
using System.Linq;
using Godot;
using GodotAnalysers;

[SceneReference("Main.tscn")]
public partial class Main
{
    [Export]
    public PackedScene PlanetScene;

    [Export]
    public PackedScene DronesScene;

    private Random r = new Random();

    private const int PlanetSize = 50;
    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        // For debug purposes all achievements can be reset
        this.di.localAchievementRepository.ResetAchievements();

        // See achievements definitions in gd-achievements/achievements.json
        // this.achievementNotifications.UnlockAchievement("MyFirstAchievement");
        this.achievementList.ReloadList();

        for (var i = 0; i < 10; i++)
        {
            int dronesCount = r.Next(50);

            Vector2 position;
            bool found;
            do
            {
                position = new Vector2(r.Next(480 - 2 * PlanetSize) + PlanetSize, r.Next(650 - 2 * PlanetSize) + PlanetSize);
                found = this.GetTree().GetNodesInGroup(Groups.Planet)
                        .Cast<Planet>()
                        .Where(a => (a.Position - position).Length() < 100)
                        .Any();
            } while (found && i > 0);
            var isNeutral = i > 0 && r.Next(100) > 10;

            var planet = PlanetScene.Instance<Planet>();
            planet.Position = position;
            planet.DronesCount = dronesCount;
            planet.PlayerId = isNeutral ? Constants.PlayerNeutralId : Constants.PlayerAllyId;
            this.AddChild(planet);

            var planet2 = PlanetScene.Instance<Planet>();
            planet2.Position = new Vector2(480, 650) - position;
            planet2.DronesCount = dronesCount;
            planet2.PlayerId = isNeutral ? Constants.PlayerNeutralId : Constants.PlayerEnemyId;
            this.AddChild(planet2);
        }
    }

    private bool isDragging = false;
    private Planet draggingFrom = null;
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
    }
}
