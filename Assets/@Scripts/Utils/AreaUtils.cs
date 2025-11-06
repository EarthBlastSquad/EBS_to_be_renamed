using UnityEngine;

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
    }
}