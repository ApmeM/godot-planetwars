using GodotAnalysers;
using Godot;
using GodotTemplate.Presentation.Utils;

[SceneReference("SelectedPlanetDetails.tscn")]
public partial class SelectedPlanetDetails
{
    private Planet details;

    public Planet Details
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
                this.growSpeedIncreaseButton.Visible = details.PlayerId == Constants.PlayerAllyId;
            }

            this.Visible = details != null;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.growSpeedIncreaseButton.Connect(CommonSignals.Pressed, this, nameof(SpeedUp));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (this.Details != null)
        {
            this.dronesCountValueLabel.Text = this.Details.DronesCount.ToString();
            this.growSpeedValueLabel.Text = this.Details.GrowSpeed.ToString();
            this.growSpeedIncreaseButton.Disabled = this.Details.DronesCount < this.Details.GrowSpeed * 10;
        }
    }

    private void SpeedUp()
    {
        if (this.Details == null)
        {
            return;
        }

        if (this.Details.DronesCount >= this.Details.GrowSpeed * 10)
        {
            this.Details.DronesCount -= (int)(this.Details.GrowSpeed * 10);
            this.Details.GrowSpeed++;
        }
    }
}
