using System;
using System.Linq;
using Godot;
using GodotTemplate.LevelSelector;
using GodotTemplate.Presentation.Utils;

namespace GodotTemplate.Levels
{
    public class Level2 : ILevelToSelect
    {
        private readonly Random r = new Random();
        private const int PlanetSize = 50;
        private const int PlayersCount = 3;

        public string Name => "Level 2";
        public void Init(Game game)
        {
            game.GameField.ClearChildren();

            for (var i = 0; i < 5; i++)
            {
                int dronesCount = r.Next(50);

                Vector2 position;
                bool found;
                do
                {
                    position = new Vector2(r.Next(480 - 2 * PlanetSize) + PlanetSize, r.Next(480 - 2 * PlanetSize) + PlanetSize);
                    found = game.GameField.GetTree().GetNodesInGroup(Groups.Planet)
                            .Cast<Planet>()
                            .Where(a => (a.Position - position).Length() < 100)
                            .Any();
                } while (found && i > 0);
                var isNeutral = i > 0 && r.Next(100) > 10;

                var vector = position - new Vector2(240, 400);

                for (var p = 0; p < PlayersCount; p++)
                {
                    var loc = vector.Rotated(p * 2 * (float)Math.PI / 3);

                    var planet = game.PlanetScene.Instance<Planet>();
                    planet.Position = loc + new Vector2(240, 240);
                    planet.DronesCount = dronesCount;
                    planet.PlayerId = isNeutral ? Constants.PlayerNeutralId : p + 1;
                    game.GameField.AddChild(planet);
                }
            }
        }
    }
}