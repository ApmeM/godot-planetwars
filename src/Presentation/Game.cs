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

    public Node2D GameField => this.gameField;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._Input(@event);
        if (!(@event is InputEventMouseButton mouse))
        {
            return;
        }

        if (mouse.ButtonIndex == (int)ButtonList.Left && mouse.Pressed)
        {
            this.planetDetails.Details = this.GetTree().GetNodesInGroup(Groups.Selectable)
                .Cast<ISelectable>()
                .Where(a => a.IsClicked(mouse.Position))
                .FirstOrDefault();
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!this.Visible)
        {
            return;
        }

        var playersInTheGame = this.GetTree().GetNodesInGroup(Groups.Planet)
                .Cast<Planet>()
                .Select(a => a.PlayerId)
                .Where(a => a != Constants.PlayerNeutralId)
                .ToHashSet();

        if (playersInTheGame.Count == 1)
        {
            EmitSignal(nameof(EndGame), playersInTheGame.First());
            this.achievementNotifications.UnlockAchievement("MyFirstAchievement");
            return;
        }
    }

    public void MakeConnection(int playerId, Planet from, Planet to)
    {
        if (to == null || to == from)
        {
            return;
        }

        var connection = PlanetConnectionScene.Instance<PlanetConnection>();
        connection.PlayerId = playerId;
        connection.From = from;
        connection.To = to;
        this.GameField.AddChild(connection);
    }

    public void SendDrones(int playerId, Planet from, Planet to, int count)
    {
        if (from.PlayerId != playerId)
        {
            return;
        }

        var drones = this.DronesScene.Instance<Drones>();
        this.GameField.AddChild(drones);
        drones.Go(from, to, count);
    }
}
