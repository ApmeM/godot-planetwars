using System;
using Godot;
using GodotAnalysers;

[SceneReference("Drones.tscn")]
public partial class Drones
{
    private int playerId = 0;
    private bool playerIdDirty = true;
    [Export]
    public int PlayerId
    {
        get => playerId;
        set
        {
            this.playerId = value;
            this.playerIdDirty = true;
        }
    }

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
            this.dronesCountDirty = false;
        }

        if (this.playerIdDirty)
        {
            this.nodeNeutral.Visible = this.playerId == Constants.PlayerNeutralId;
            this.nodePlayer1.Visible = this.playerId == 1;
            this.nodePlayer2.Visible = this.playerId == 2;
            this.nodePlayer3.Visible = this.playerId == 3;
            this.playerIdDirty = false;
        }
    }

    public void Go(Planet from, Planet to, int dronesToSend)
    {
        this.DronesCount = dronesToSend;
        this.To = to;
        this.Position = from.Position;
        this.PlayerId = from.PlayerId;
        from.DronesCount -= dronesToSend;

        var tween = this.CreateTween();
        tween.TweenProperty(this, "position", To.Position, (from.Position - to.Position).Length() / 50);
        tween.TweenCallback(this, nameof(DronesArrived));
    }

    private void DronesArrived()
    {
        var drones = this;
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
}
