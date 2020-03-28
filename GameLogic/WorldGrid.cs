using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace MiniEngine.GameLogic
{
    public sealed class WorldGrid
    {
        private readonly Grid Grid;
        private readonly PathFinder PathFinder;
        private readonly Entity[,] Reservations;
        private readonly Velocity MaximumVelocity;
        private readonly Size CellSize;
        private readonly Vector3 Offset;
        private readonly Vector3 CellOffset;

        public WorldGrid(int columns, int rows, float cellDimensions, float traversalVelocity, Vector3 offset)
        {
            var gridSize = new GridSize(columns, rows);
            this.CellSize = new Size(Distance.FromMeters(cellDimensions), Distance.FromMeters(cellDimensions));
            this.MaximumVelocity = Velocity.FromMetersPerSecond(traversalVelocity);
            // TODO: if we create a grid with lateral and diagonal connections paths look a lot better
            // however we have to take into account 'crossing over' other cells when reserving.            
            this.Grid = Grid.CreateGridWithLateralConnections(gridSize, this.CellSize, this.MaximumVelocity);
            this.PathFinder = new PathFinder();
            this.Reservations = new Entity[columns, rows];

            this.Offset = offset;
            this.CellOffset = new Vector3(this.CellSize.Width.Meters * 0.5f, 0, this.CellSize.Height.Meters * 0.5f);
        }

        public bool Reserve(Entity entity, GridPosition gridPosition)
        {
            var holder = this.Reservations[gridPosition.X, gridPosition.Y];
            if (holder == default || holder == entity)
            {
                this.Reservations[gridPosition.X, gridPosition.Y] = entity;
                return true;
            }

            return false;
        }

        public void Free(GridPosition gridPosition)
            => this.Reservations[gridPosition.X, gridPosition.Y] = default;

        public Path PlanPath(GridPosition from, GridPosition to)
        {
            // TODO: maybe plan paths from world positions, remove the last node and use the given world position?
            var path = this.PathFinder.FindPath(from, to, this.Grid, this.MaximumVelocity);

            var waypoints = new List<Vector3>(path.Edges.Count + 1);

            waypoints.Add(this.ToWorldPositionCentered(from));
            if (path.Edges.Count > 0)
            {
                for (var i = 0; i < path.Edges.Count; i++)
                {
                    var position = path.Edges[i].End.Position;
                    waypoints.Add(new Vector3(position.X, 0, position.Y) + this.CellOffset + this.Offset);
                }
            }

            return new Path(waypoints);
        }

        public GridPosition ToGridPosition(Vector3 worldPosition)
        {
            var x = (int)((worldPosition.X / this.CellSize.Width.Meters) - this.Offset.X);
            var y = (int)((worldPosition.Z / this.CellSize.Height.Meters) - this.Offset.Z);

            return new GridPosition(x, y);
        }

        public Vector3 ToWorldPosition(GridPosition gridPosition)
        {
            var x = gridPosition.X * this.CellSize.Width.Meters;
            var y = gridPosition.Y * this.CellSize.Height.Meters;

            return new Vector3(x, 0, y) + this.Offset;
        }

        public Vector3 ToWorldPositionCentered(GridPosition gridPosition)
            => this.ToWorldPosition(gridPosition) + this.CellOffset;
    }
}
