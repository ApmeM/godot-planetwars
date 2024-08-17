using System;
using System.Linq;
using Godot;
using GodotTemplate.LevelSelector;
using GodotTemplate.Presentation.Utils;

namespace GodotTemplate.Levels
{
    public class Level1 : ILevelToSelect
    {
        private readonly Random r = new Random();
        private const int PlanetSize = 50;

        public string Name => "Level 1";
        public void Init(Game game)
        {
            game.GameField.ClearChildren();

            for (var i = 0; i < 8; i++)
            {
                int dronesCount = r.Next(50);

                Vector2 position;
                bool found;
                do
                {
                    position = new Vector2(r.Next(480 - 2 * PlanetSize) + PlanetSize, r.Next(650 - 2 * PlanetSize) + PlanetSize);
                    found = game.GetTree().GetNodesInGroup(Groups.Planet)
                            .Cast<Planet>()
                            .Where(a => (a.Position - position).Length() < 100)
                            .Any();
                } while (found && i > 0);
                var isNeutral = i > 0 && r.Next(100) > 10;

                var planet = game.PlanetScene.Instance<Planet>();
                planet.Position = position;
                planet.DronesCount = dronesCount;
                planet.PlayerId = isNeutral ? Constants.PlayerNeutralId : Constants.PlayerAllyId;
                game.GameField.AddChild(planet);

                var planet2 = game.PlanetScene.Instance<Planet>();
                planet2.Position = new Vector2(480, 650) - position;
                planet2.DronesCount = dronesCount;
                planet2.PlayerId = isNeutral ? Constants.PlayerNeutralId : Constants.PlayerEnemyId;
                game.GameField.AddChild(planet2);
            }
        }
    }
}