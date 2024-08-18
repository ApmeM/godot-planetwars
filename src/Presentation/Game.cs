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

    [Export]
    public PackedScene PlanetConnectionScene;

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
    private PlanetConnection newConnection = null;

    public Node2D GameField => this.gameField;

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
                    .Where(a => a.PlayerId == Constants.PlayerAllyId)
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
                    .Where(a => a.PlayerId == Constants.PlayerAllyId)
                    .Where(a => a.IsClicked(mouse.Position))
                    .FirstOrDefault();

                if (draggingFrom != null && draggingFrom is Planet planet)
                {
                    newConnection = PlanetConnectionScene.Instance<PlanetConnection>();
                    newConnection.PlayerId = planet.PlayerId;
                    newConnection.DronesScene = this.DronesScene;
                    newConnection.From = planet;
                    this.AddChild(newConnection);
                }

                this.planetDetails.Details = draggingFrom;
            }
            else if (newConnection != null)
            {
                newConnection.Active = true;
                if (newConnection.To == null || newConnection.To == newConnection.From)
                {
                    newConnection.QueueFree();
                }
                newConnection = null;
            }

            isDragging = newConnection != null;
        }
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
            newConnection.To = this.GetTree().GetNodesInGroup(Groups.Planet)
                .Cast<Planet>()
                .Where(a => a.IsClicked(mousePosition))
                .FirstOrDefault();
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
