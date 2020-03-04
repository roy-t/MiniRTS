using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace MiniEngine.GameLogic
{
    public sealed class DumbMovementLogic
    {
        private readonly Grid Grid;
        private readonly PathFinder PathFinder;
        private readonly GridSize GridSize = new GridSize(20, 20);
        private readonly Size CellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
        private readonly Vector2 Offset;

        public DumbMovementLogic()
        {
            this.Grid = Grid.CreateGridWithLateralAndDiagonalConnections(this.GridSize, this.CellSize, Velocity.FromKilometersPerHour(50));
            this.PathFinder = new PathFinder();

            this.Offset = new Vector2
                (
                    -(this.GridSize.Columns * this.CellSize.Width.Meters * 0.5f),
                    -(this.GridSize.Rows * this.CellSize.Height.Meters * 0.5f)
                );
        }

        public List<Vector2> PlanPath(int x0, int y0, int x1, int y1)
        {
            var pathFinder = new PathFinder();
            var path = pathFinder.FindPath(new GridPosition(x0, y0), new GridPosition(x1, y1), this.Grid);

            var checkpoints = new List<Vector2>();

            if (path.Edges.Count > 0)
            {
                for (var i = 0; i < path.Edges.Count; i++)
                {
                    var current = path.Edges[i];
                    var currentX = current.End.Position.X;
                    var currentY = current.End.Position.Y;
                    checkpoints.Add(new Vector2(currentX * this.CellSize.Width.Meters, currentY * this.CellSize.Height.Meters) + this.Offset);
                }
            }
            else
            {
                checkpoints.Add(new Vector2(x0 * this.CellSize.Width.Meters, y0 * this.CellSize.Height.Meters) + this.Offset);
            }

            return checkpoints;
        }
    }
}
