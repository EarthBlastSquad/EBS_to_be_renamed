using UnityEngine;
using Utils.Defines;

namespace Utils
{
    public static class AreaUtils
    {
        public static bool IsPointInSquareBoundary(Vector2Int areaStart, Vector2Int areaEnd, Vector2Int point)
        {
            if (point.x < areaStart.x || point.x > areaEnd.x)
            {
                return false;
            }

            if (point.y < areaStart.y || point.y > areaEnd.y)
            {
                return false;
            }

            return true;
        } 
        
        public static Vector2Int CalculateRotation(Vector2Int pos, Facing rotation)
        {

            if(rotation == Facing.UP)
            {
                return new Vector2Int(-pos.y, pos.x);
            }

            if(rotation == Facing.LEFT)
            {
                return -pos;
            }

            if(rotation == Facing.DOWN)
            {
                return new Vector2Int(pos.y, -pos.x);
            }

            return pos;
        }
    }
}