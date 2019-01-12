using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Heap;
using System;
using Jext;

namespace Movement
{
    public class Plotter<T, U> where T : IMoveable<U> where U : class, IHeapable<U>, INodeable<U>
    {
        private U[,] grid;
        private Heap<U> open;
        private HashSet<U> closed = new HashSet<U>();

        public Plotter(U[,] grid)
        {
            this.grid = grid;
            open = new Heap<U>(grid.GetLength(0) * grid.GetLength(1));
        }

        public void Get(T moveable, Vector2Int to, List<U> path)
        {
            open.Clear();
            closed.Clear();

            moveable.Position.Parent = null;
            open.Add(moveable.Position);

            U current = null;

            Action<int, int> tryAddNeighbour = delegate (int x, int y)
            {
                if (grid.IsOutOfBounds(x, y))
                    return;
                U node = grid[x, y];
                if (!moveable.Walkable(node))
                    return;
                if (closed.Contains(node))
                    return;
                if (open.Contains(node))
                    return;
                Vector2Int position = new Vector2Int(x, y);

                node.Parent = current;
                node.G = Vector2.Distance(new Vector2Int(node.Position.x, node.Position.y), position);
                node.H = Vector2Int.Distance(to, position);

                open.Add(node);
            };

            while (open.Count > 0)
            {
                current = open.Get();
                closed.Add(current);

                if (current.Position.Equals(to))
                    goto OnFound;

                if (moveable.HorizontalMovement)
                {
                    tryAddNeighbour(current.Position.x, current.Position.y + 1);
                    tryAddNeighbour(current.Position.x + 1, current.Position.y);
                    tryAddNeighbour(current.Position.x, current.Position.y - 1);
                    tryAddNeighbour(current.Position.x - 1, current.Position.y);
                }

                if (moveable.VerticalMovement)
                {
                    tryAddNeighbour(current.Position.x + 1, current.Position.y + 1);
                    tryAddNeighbour(current.Position.x + 1, current.Position.y - 1);
                    tryAddNeighbour(current.Position.x - 1, current.Position.y - 1);
                    tryAddNeighbour(current.Position.x - 1, current.Position.y + 1);
                }
            }

            return;

            OnFound:
            U parent = current.Parent;

            while (parent != null)
            {
                path.Add(parent);
                parent = parent.Parent;
            }
        }

        private float GetDistance(U node)
        {
            float distance = node.G + node.H;
            U parent = node.Parent;

            while (parent != null)
            {
                distance += parent.G + parent.H;
                parent = parent.Parent;
            }

            return distance;
        }
    }

    public interface IMoveable<T>
    {
        T Position { get; }
        bool HorizontalMovement { get; }
        bool VerticalMovement { get; }
        bool Walkable(T node);
    }

    public interface INodeable<T>
    {
        T Parent { get; set; }
        Vector2Int Position { get; set; }

        // I'm using a heap instead of a stack for performance reasons
        int HeapIndex { get; set; }

        // Pathfinding
        float G { get; set; }
        float H { get; set; }
    }
}