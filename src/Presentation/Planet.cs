using System;
using Godot;
using GodotAnalysers;

[SceneReference("Planet.tscn")]
[Tool]
public partial class Planet : IMinimapElement
{
    public bool VisibleOnBorder => true;

    public Sprite Sprite => this.planetSprite;

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
        }
    }

    public Rect2 GetRect() => this.planetSprite.GetRect();
}
