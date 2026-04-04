using Contents.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Controller
{
    public class GridController : MonoBehaviour
    {
        private Tilemap _tileMap;
        private GameObject _clickedCellHighlighter;
        private GameObject _lockedAreaShadow;
        private void Awake()
        {
            _tileMap = GetComponent<Tilemap>();
            if (_tileMap is null)
            {
                Debug.LogError("tilemap not found in grid");
            }
        }

        private void SetupLockedShadows(int width, int height)
        {
            if (_lockedAreaShadow is null)
            {
                _lockedAreaShadow = Manager.Managers.Instance.ResourceManager.Instantiate("LockedAreaShadow", transform);
            }
            
            _lockedAreaShadow.transform.localScale = new Vector3(width, height, 1);
            _lockedAreaShadow.transform.position = _tileMap.CellToWorld(Vector3Int.zero);
        }

        public bool SetupGridTiles(GridCellData[,] cellInfo,string gridImgName)
        {
            int width = cellInfo.GetLength(0);
            int height = cellInfo.GetLength(1);
            Tile tile = Manager.Managers.Instance.ResourceManager.Load<Tile>(gridImgName);
            if (tile is null)
            {
                Debug.LogError("tile not found");
                return false;
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    
                    _tileMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }

            SetupLockedShadows(width, height);

            return true;
        }

        public void SetLockedAreaShadowXPos(int xPos)
        {
            if (_lockedAreaShadow is null)
            {
                Debug.LogError("shadow not created");
                return;
            }
            _lockedAreaShadow.transform.position = _tileMap.CellToWorld(new Vector3Int(xPos,0,0));
        }

        public void DeltaLockedAreaShadowXScale(int xLen)
        {
            var scale = _lockedAreaShadow.transform.localScale;
            scale.x += xLen;
            _lockedAreaShadow.transform.localScale = scale;
        }

        public void SetHighlightAt(Vector3Int pos)
        {
            if (_clickedCellHighlighter is null)
            {
                _clickedCellHighlighter = Manager.Managers.Instance.ResourceManager.Instantiate("ClickedCellHighlighter");
                _clickedCellHighlighter.transform.SetParent(transform);
                //null체킹 하려 했는데, 안해도 될듯?
            }

            _clickedCellHighlighter.transform.position = _tileMap.CellToWorld(pos);
            //좌표 체킹 생략
        }

        public void PlacePieceAt(Vector3Int pos, Transform pieceTransform)
        {
            if (pieceTransform is null)
            {
                return;
            }

            pieceTransform.position = _tileMap.CellToWorld(pos);//좌표체킹은 manager에서 하고 넘어와서 검사 x
        }
    }
}
//이번에는 딱 클릭하고 하이라이팅까지만
//그 이상은 아직 생각하지 말자
//너무 깊게 생각하면 오히려 시작도 못한다
//해상도도 이번주에 가서 물어볼 때 생각해