using Contents.Grid;
using Controller;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class GridManager : MonoBehaviour
    {
        private GridController _gridController;
        private Grid _grid;
        private GridCellData[,] _datas;
        private bool _isInit = false;
        private Vector2Int _lastSelectedPos;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            if (_grid is null)
            {
                Debug.LogError("grid not found in grid");
            }

            _gridController = GetComponentInChildren<GridController>();
            if (_gridController is null)
            {
                Debug.LogError("grid controller not found in child");
            }
        }

        public bool Init()
        {
            if (_isInit)
            {
                return false;
            }
            _datas = new GridCellData[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];
            _gridController.SetupGridTiles(_datas);
            _isInit = true;
            return true;
        }

        public bool IsItValidCellPos(Vector2Int pos)
        {
            return _isInit && Utils.AreaUtils.IsPointInSquareBoundary(Vector2Int.zero, new Vector2Int((int)MapMaxCellCnt.MAX_WIDTH - 1, (int)MapMaxCellCnt.MAX_HEIGHT - 1), pos);
        }

        public Vector2Int GetLastSelectedPos()
        {
            return _lastSelectedPos;
        }

        public int GetCellImgNumAt(Vector2Int pos)
        {
            if (IsItValidCellPos(pos) == false)
            {
                Debug.LogError("Wrong cell pos");
                return -1;
            }

            return _datas[pos.x, pos.y].cellImgNumber;
        }

        public bool IsItLocked(Vector2Int pos)
        {
            if (IsItValidCellPos(pos) == false)
            {
                Debug.LogError("Wrong cell pos");
                return true;
            }

            return _datas[pos.x, pos.y].isLocked;
        }

        public bool TryGetPlacedPiece(Vector2Int pos, out GameObject outPiece)
        {
            outPiece = null;
            if (IsItValidCellPos(pos) == false)
            {
                Debug.LogError("Wrong cell pos");
                return false;
            }

            outPiece = _datas[pos.x, pos.y].nowHoldingPiece;
            return true;
        }
    }

}
/*
딱 이미지 셋업까지만

lock tile관련은 다음 업데이트 때
기물 설치도 다음 업데이트 때

*/