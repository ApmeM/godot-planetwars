using System;
using Godot;
using GodotAnalysers;

[SceneReference("Planet.tscn")]
public partial class Planet : IMinimapElement, ISelectable
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

    private bool selected = false;
    private bool selectedDirty = true;
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
        this.AddToGroup(Groups.Selectable);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        var parent = this.GetParent().GetParent<Game>();

        if (this.dronesCountDirty)
        {
            this.dronesCountLabel.Text = this.dronesCount.ToString();
            this.dronesCountDirty = false;
        }

        if (this.playerIdDirty)
        {
            this.planetNeutralSprite.Visible = this.playerId == Constants.PlayerNeutralId;
            this.planetPlayer1Sprite.Visible = this.playerId == 1;
            this.planetPlayer2Sprite.Visible = this.playerId == 2;
            this.planetPlayer3Sprite.Visible = this.playerId == 3;
            this.playerIdDirty = false;
        }

        if (selectedDirty)
        {
            this.planetSelectedSprite.Visible = this.selected;
        }

        if (dronesCount < DronesMaxCount && this.playerId != Constants.PlayerNeutralId)
        {
            this.GrowTimeout += this.GrowSpeed * delta;
            if (this.GrowTimeout > 1)
            {
                this.DronesCount++;
                this.GrowTimeout--;
            }
        }
    }

    public bool IsClicked(Vector2 position)
    {
        return this.planetNeutralSprite.GetRect().HasPoint(this.ToLocal(position));
    }
}
