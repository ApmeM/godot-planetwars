using Godot;
using System;
using System.Linq;

public class OpponentEasy : Node
{
    [Export]
    public NodePath Field;

    [Export]
    public float ThinkDelay { get; set; }

    [Export]
    public int PlayerId { get; set; }

    public float CurrentThinkDelay;

    private Random r = new Random();

    public override void _Process(float delta)
    {
        base._Process(delta);
        var fld = this.GetNode(Field);

        var myPlanets = this.GetTree().GetNodesInGroup(Groups.Planet)
            .Cast<Planet>()
            .Where(a => a.PlayerId == this.PlayerId)
            .ToList();
        var otherPlanets = this.GetTree().GetNodesInGroup(Groups.Planet)
            .Cast<Planet>()
            .Where(a => a.PlayerId != this.PlayerId)
            .ToList();
        var connections = this.GetTree().GetNodesInGroup(Groups.PlanetConnection)
            .Cast<PlanetConnection>()
            .Where(a => a.PlayerId == this.PlayerId)
            .ToList();
        var connectedPlanets = connections
            .Select(a => a.From)
            .ToHashSet();

        foreach (var conn in connections)
        {
            if (conn.From.PlayerId == conn.To.PlayerId)
            {
                conn.QueueFree();
            }
        }

        foreach (var planet in myPlanets)
        {
            if (connectedPlanets.Contains(planet))
            {
                continue;
            }

            var target = otherPlanets[r.Next(otherPlanets.Count - 1)];

            var game = this.GetParent<Game>();
            game.MakeConnection(this.PlayerId, planet, target);
        }
    }
}
