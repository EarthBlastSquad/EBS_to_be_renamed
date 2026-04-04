using Data;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;
namespace ObjectPool
{
    public class RangePreview : MonoBehaviour
    {
        private Mesh _mesh;
        private RectInt _gridRect = new RectInt(0, 0, (int)Utils.Defines.MapMaxCellCnt.MAX_WIDTH, (int)Utils.Defines.MapMaxCellCnt.MAX_HEIGHT);

        void Awake()
        {
            _mesh = new Mesh();
            _mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        public void ShowAttackRange(IReadOnlyList<Vector2Int> localPattern, Vector2Int placedPos, Facing facing)
        {
            List<Vector2Int> worldCells = ConvertToWorldCells(localPattern, placedPos, facing);
            gameObject.SetActive(true);
            Draw(worldCells);
        }

        public void Hide()
        {
            _mesh.Clear();
            gameObject.SetActive(false);
        }
        private List<Vector2Int> ConvertToWorldCells(IReadOnlyList<Vector2Int> localPattern, Vector2Int placedPos, Facing facing)
        {
            List<Vector2Int> result = new List<Vector2Int>(localPattern.Count);

            for (int i = 0; i < localPattern.Count; i++)
            {
                Vector2Int local = localPattern[i];
                Vector2Int rotated = ApplyFacing(local, facing);
                if (!_gridRect.Contains(placedPos + rotated))
                    continue;
                result.Add(placedPos + rotated);
            }

            return result;
        }

        private Vector2Int ApplyFacing(Vector2Int v, Facing facing)
        {
            switch (facing)
            {
                case Facing.UP: return new Vector2Int(-v.y, v.x);
                case Facing.RIGHT: return v;
                case Facing.DOWN: return new Vector2Int(v.y, -v.x);
                case Facing.LEFT: return new Vector2Int(-v.x, -v.y);
            }
            return v;
        }
        private void Draw(IReadOnlyList<Vector2Int> cells)
        {
            _mesh.Clear();

            int cellCount = cells.Count;

            Vector3[] vertices = new Vector3[cellCount * 4];
            int[] triangles = new int[cellCount * 6];

            float z = -0.01f;

            for (int i = 0; i < cellCount; i++)
            {
                Vector2Int cell = cells[i];

                int vertice = i * 4;
                int triangle = i * 6;

                vertices[vertice + 0] = new Vector3(cell.x, cell.y, z);
                vertices[vertice + 1] = new Vector3(cell.x + 1, cell.y, z);
                vertices[vertice + 2] = new Vector3(cell.x, cell.y + 1, z);
                vertices[vertice + 3] = new Vector3(cell.x + 1, cell.y + 1, z);

                triangles[triangle + 0] = vertice + 0;
                triangles[triangle + 1] = vertice + 2;
                triangles[triangle + 2] = vertice + 1;

                triangles[triangle + 3] = vertice + 2;
                triangles[triangle + 4] = vertice + 3;
                triangles[triangle + 5] = vertice + 1;
            }
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateBounds();
        }

    }
}