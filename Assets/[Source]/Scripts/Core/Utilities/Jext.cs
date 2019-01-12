using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Pathfinding;

namespace Jext
{
    public static class Jext
    {
        public static Vector2Int ConvertToGridPosition(this Vector3 pos)
        {
            return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        }

        public static Vector3 ConvertToWorldPosition(this Vector2Int pos, float y)
        {
            return new Vector3(pos.x, y, pos.y);
        }

        public static void GetCircle<T>(this T[,] level, List<T> circle, Vector2Int from, int range)
        {
            int threshold = range * range;

            for (int i = -range; i <= range; i++)
                for (int j = -range; j <= range; j++)
                    if (!level.IsOutOfBounds(i + from.x, j + from.y))
                        if (i * i + j * j < threshold)
                            circle.Add(level[i + from.x, j + from.y]);
        }

        public static void GetSquire<T>(this T[,] level, List<T> squire, Vector2Int from, Vector2Int shape)
        {
            for (int x = 0; x < shape.x; x++)
                for (int y = 0; y < shape.y; y++)
                    squire.Add(level[x + from.x, y + from.y]);
        }

        public static bool IsOutOfBounds<T>(this T[,] level, int x, int y)
        {
            return x < 0 || y < 0 || x >= level.GetLength(0) || y >= level.GetLength(1);
        }

        public static bool Intersects(Vector2Int aPos, Vector2Int aSize, Vector2Int bPos, Vector2Int bSize)
        {
            Vector2Int aLeftDown = aPos, aRightDown = aPos + new Vector2Int(aSize.x, 0),
                aLeftUp = aPos + new Vector2Int(0, aSize.y), aRightUp = aPos + aSize;

            if (Intersects(bPos, bSize, aLeftDown))
                return true;
            if (Intersects(bPos, bSize, aRightDown))
                return true;
            if (Intersects(bPos, bSize, aLeftUp))
                return true;
            if (Intersects(bPos, bSize, aRightUp))
                return true;
            return false;
        }

        public static bool Intersects(Vector2Int pos, Vector2Int size, Vector2Int point)
        {
            return point.x >= pos.x && point.x <= pos.x + size.x && point.y >= pos.y && point.y <= pos.y + size.y;
        }

        public static void Initialize<T>(this T[,] level) where T : INodeable<T>, new()
        {
            int xLength = level.GetLength(0), yLength = level.GetLength(1);

            for (int x = 0; x < xLength; x++)
                for (int y = 0; y < yLength; y++)
                {
                    level[x, y] = new T();
                    level[x, y].Position = new Vector2Int(x, y);
                }
        }

        public static Vector2Int ConvertMovePositionToGridPosition(this Move move, Vector2Int movePosition)
        {
            Vector2Int center = move.Center();
            return new Vector2Int(movePosition.x - center.x, move.footPrint.GetLength(1) - movePosition.y - center.y);
        }

        public static Vector2Int Center(this Move move)
        {
            return new Vector2Int(move.footPrint.GetLength(0) / 2, move.footPrint.GetLength(1) / 2);
        }
    }
}
