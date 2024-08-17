using GodotAnalysers;
using Godot;
using GodotTemplate.Presentation.Utils;
using System;

[SceneReference("SelectedPlanetDetails.tscn")]
public partial class SelectedPlanetDetails
{
    private ISelectable details;

    public ISelectable Details
    {
        get => details;
        set
        {
            if (details != null)
            {
                details.Selected = false;
            }

            details = value;

            if (details != null)
            {
                details.Selected = true;

                this.planetContainer.Visible = details is Planet;
                this.connectionContainer.Visible = details is PlanetConnection;

                this.growSpeedIncreaseButton.Visible = details.PlayerId == Constants.PlayerAllyId;
                this.deleteConnectionButton.Visible = details.PlayerId == Constants.PlayerAllyId;
            }

            this.Visible = details != null;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.growSpeedIncreaseButton.Connect(CommonSignals.Pressed, this, nameof(SpeedUp));
        this.deleteConnectionButton.Connect(CommonSignals.Pressed, this, nameof(DeleteConnection));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (this.Details != null)
        {
            if (this.Details is Planet planet)
            {
                this.dronesCountValueLabel.Text = planet.DronesCount.ToString();
                this.growSpeedValueLabel.Text = planet.GrowSpeed.ToString();
                this.growSpeedIncreaseButton.Disabled = planet.DronesCount < planet.GrowSpeed * 10;
            }
        }
    }

    private void SpeedUp()
    {
        var planet = this.Details as Planet;
        if (planet == null)
        {
            return;
        }

        if (planet.DronesCount >= planet.GrowSpeed * 10)
        {
            planet.DronesCount -= (int)(planet.GrowSpeed * 10);
            planet.GrowSpeed++;
        }
    }

    private void DeleteConnection()
    {
        var connection = this.Details as PlanetConnection;
        if (connection == null)
        {
            return;
        }

        connection.QueueFree();
        this.Details = null;
    }
}
