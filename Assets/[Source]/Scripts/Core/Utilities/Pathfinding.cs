using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Heap;
using Jext;

namespace Core.Pathfinding
{
    public class Pathfinding
    {
        private Node[,] grid;
        private Heap<Node> open;
        private HashSet<Node> closed = new HashSet<Node>();

        public Pathfinding(Node[,] grid)
        {
            this.grid = grid;
            open = new Heap<Node>(grid.GetLength(0) * grid.GetLength(1));
        }

        public void Get(Combatant combatant, Vector2Int to, List<Node> path)
        {
            Node fromNode = grid[combatant.Node.Position.x, combatant.Node.Position.y], 
                toNode = grid[to.x, to.y],
                current = null, 
                child;
            Vector2Int gridSize, 
                convertedPosition;

            open.Clear();
            closed.Clear();

            fromNode.Parent = null;
            open.Add(fromNode);

            while (open.Count > 0)
            {
                current = open.Get();
                closed.Add(current);

                if (current.Position == to)
                    break;

                foreach(Move move in combatant.moveSet.moves)
                {
                    gridSize = new Vector2Int(move.footPrint.GetLength(0), move.footPrint.GetLength(1));

                    for (int x = 0; x < gridSize.x; x++)
                        for (int y = 0; y < gridSize.y; y++)
                        {
                            if (!move.footPrint[x, y])
                                continue;
                            convertedPosition = new Vector2Int(current.Position.x + x, current.Position.y + y);
                            child = grid[convertedPosition.x, convertedPosition.y];

                            if (closed.Contains(child))
                                continue;
                            if (open.Contains(child))
                                continue;                         
                            if (grid.IsOutOfBounds(convertedPosition.x, convertedPosition.y))
                                continue;
                            
                            child.Parent = current;
                            open.Add(child);
                        }
                }
            }
            
            while(current.Parent != null)
            {
                path.Add(current);
                current = current.Parent;
            }
        }
    }

    public interface IMoveable<T>
    {
        T Node { get; }
        bool HorizontalMovement { get; }
        bool VerticalMovement { get; }
        bool Walkable(T node);
    }

    public interface INodeable<T>
    {
        T Parent { get; set; }
        Move Move { get; set; }
        Vector2Int Position { get; set; }

        int HeapIndex { get; set; }

        float G { get; set; }
        float H { get; set; }
    }
}