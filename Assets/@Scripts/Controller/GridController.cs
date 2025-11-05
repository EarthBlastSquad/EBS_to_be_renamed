using Contents.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Controller
{
    public class GridController : MonoBehaviour
    {
        [SerializeField]
        private string _tileName = "grid_cell_tile";
        private Tilemap _tileMap;
        private void Awake()
        {
            _tileMap = GetComponent<Tilemap>();
            if (_tileMap is null)
            {
                Debug.LogError("tilemap not found in grid");
            }
        }

        public bool SetupGridTiles(GridCellData[,] cellInfo)
        {
            int width = cellInfo.GetLength(0);
            int height = cellInfo.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = Manager.Managers.Instance.ResourceManager.Load<Tile>($"{_tileName}_{cellInfo[x, y].cellImgNumber}");
                    if (tile is null)
                    {
                        Debug.LogError("tile not found");
                        return false;
                    }
                    _tileMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }

            return true;
        }
    }
}
//이번에는 딱 클릭하고 하이라이팅까지만
//그 이상은 아직 생각하지 말자
//너무 깊게 생각하면 오히려 시작도 못한다
//해상도도 이번주에 가서 물어볼 때 생각해