using System;
using Godot;
using GodotAnalysers;

[SceneReference("Planet.tscn")]
public partial class Planet : IMinimapElement
{
    public bool VisibleOnBorder => true;

    public Sprite Sprite => this.planetNeutralSprite;

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

    [Export]
    public float GrowSpeed = 1;
    public float GrowTimeout;
    public int DronesMaxCount = 50;

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.AddToGroup(Groups.MinimapElement);
        this.AddToGroup(Groups.Planet);
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
            this.planetNeutralSprite.Visible = this.playerId == Main.PlayerNeutralId;
            this.planetEnemySprite.Visible = this.playerId == Main.PlayerEnemyId;
            this.planetAllySprite.Visible = this.playerId == Main.PlayerAllyId;
            this.playerIdDirty = false;
        }

        if (dronesCount < DronesMaxCount && this.playerId != Main.PlayerNeutralId)
        {
            this.GrowTimeout += this.GrowSpeed * delta;
            if (this.GrowTimeout > 1)
            {
                this.DronesCount++;
                this.GrowTimeout--;
            }
        }
    }

    public Rect2 GetRect() => this.planetNeutralSprite.GetRect();
}
