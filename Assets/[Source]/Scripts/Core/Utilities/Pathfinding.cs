using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Heap;
using Core.Jext;

namespace Core.Pathfinding
{
    public class Pathfinding
    {
        // A reference to the active grid
        private Node[,] grid;
        // All nodes that are currently being investigated
        private Heap<Node> open;
        // All nodes that have been investigated
        private HashSet<Node> closed = new HashSet<Node>();

        public Pathfinding(Node[,] grid)
        {
            this.grid = grid;
            // Cache open size
            open = new Heap<Node>(grid.GetLength(0) * grid.GetLength(1));
        }

        /// <summary>
        /// Get a path from combatant position to target position, based off of what the combatant can traverse
        /// </summary>
        /// <param name="combatant"></param>
        /// <param name="to"></param>
        /// <param name="path">Out path</param>
        public void GetPath(IMoveable<Node> moveable, Vector2Int to, List<Node> path)
        {
            // fromNode = start position, toNode = end position
            Node fromNode = grid[moveable.Node.Position.x, moveable.Node.Position.y], 
                toNode = grid[to.x, to.y],
                current = null, 
                child;
            Vector2Int gridSize, 
                convertedPosition;

            // Reset the lists
            open.Clear();
            closed.Clear();

            // Reset the root parent
            fromNode.Cost = 0;
            fromNode.Parent = null;

            open.Add(fromNode);

            // While there are paths that haven't been discovered
            while (open.Count > 0)
            {
                // Get best path from open and remove it, while adding it to closed
                current = open.Get();
                closed.Add(current);

                // If path has been found
                if (current.Position == to)
                    break;

                // Foreach moveset check if there are available moves
                foreach(Move move in moveable.MoveSet.moves)
                {
                    gridSize = new Vector2Int(move.footPrint.GetLength(0), move.footPrint.GetLength(1));

                    // Check moveset for possible moves
                    for (int x = 0; x < gridSize.x; x++)
                        for (int y = 0; y < gridSize.y; y++)
                        {
                            // If this is not a valid move
                            if (!move.footPrint[x, y])
                                continue;

                            convertedPosition = new Vector2Int(current.Position.x + x, current.Position.y + y);

                            if (grid.IsOutOfBounds(convertedPosition.x, convertedPosition.y))
                                continue;

                            child = grid[convertedPosition.x, convertedPosition.y];

                            // If this is not walkable in grid and this is not the goal
                            if (!moveable.Walkable(child) && child.Position != to)
                                continue;
                            // If this has been investigated already
                            if (closed.Contains(child))
                                continue;
                            // If this is being investigated but the one being investigated is better
                            if (open.Contains(child))
                                if (child.Cost <= current.Value)
                                    continue;    
                            
                            // Add child to open with correct information
                            child.Parent = current;
                            child.Cost = current.Value;
                            child.Move = move;
                            open.Add(child);
                        }
                }
            }

            // If no path has been found
            if(current.Position != to)
                return;

            // Get complete path from current node
            while(current.Parent != null)
            {
                path.Add(current);
                current = current.Parent;
            }
        }
    }

    /// <summary>
    /// This is being used for things that can be moved.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMoveable<T>
    {
        MoveSet MoveSet { get; }
        T Node { get; }
        bool HorizontalMovement { get; }
        bool VerticalMovement { get; }
        bool Walkable(T node);
    }

    /// <summary>
    /// This is being used for the ground nodes. Position needs to be set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INodeable<T>
    {
        T Parent { get; set; }
        Move Move { get; set; }
        Vector2Int Position { get; set; }

        int HeapIndex { get; set; }

        float G { get; set; }
        float H { get; set; }
        float Cost { get; set; }
    }
}