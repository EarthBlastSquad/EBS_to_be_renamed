using Contents.Grid;
using Controller;
using DG.Tweening.Core.Easing;
using InputHandler;
using System;
using System.Collections.Generic;
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
        private int _lockedAreaStartIdx = 0;
        private Queue<PieceUpdateArgs> _commandQueue = new Queue<PieceUpdateArgs>();

        public event Action<CellUpdateEventArgs> CellUpdateEvent;

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

            var gridInputHandler = GetComponent<GridInputHandler>();

            if (gridInputHandler is null)
            {
                Debug.LogError("grid input handler not found");
                return;
            }

            gridInputHandler.mouseUpGridEvent += GridMouseUpCallback;
            _commandQueue.Clear();
        }
        private void LateUpdate()
        {
            while (_commandQueue.Count > 0)
            {
                var arg = _commandQueue.Dequeue();

                if(arg.command == PieceCommandTypes.PLACE)
                {
                    arg.commandStatusCallback?.Invoke(PlacePieceAt(arg.pos, arg.instance));
                }
                else if(arg.command == PieceCommandTypes.UNPLACE)
                {
                    arg.commandStatusCallback?.Invoke(UnplacePieceAt(arg.pos));
                }
            }
        }
        public bool Init()
        {
            if (_isInit)
            {
                return false;
            }
            _datas = new GridCellData[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];

            for(int x = 0; x < (int)MapMaxCellCnt.MAX_WIDTH; x++)
            {
                for(int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
                {
                    _datas[x, y].isLocked = true;
                }
            }

            _gridController.SetupGridTiles(_datas);
            _isInit = true;
            CellUpdateEvent?.Invoke(new CellUpdateEventArgs(new Vector3Int((int)ControlValue.INVALID, (int)ControlValue.INVALID, (int)ControlValue.INVALID),false,false));
            return true;
        }

        public bool IsItValidCellPos(Vector2Int pos)
        {
            return _isInit && Utils.AreaUtils.IsPointInSquareBoundary(Vector2Int.zero, new Vector2Int((int)MapMaxCellCnt.MAX_WIDTH - 1, (int)MapMaxCellCnt.MAX_HEIGHT - 1), pos);
        }

        public bool CanPlacePiece(Vector2Int pos)
        {
            return (IsItLocked(pos) == false) && (_datas[pos.x, pos.y].nowHoldingPiece is null); // IsItValidCellPos를 이미 IsItLocked에서 수행중
        }

        public bool MoveTo(Vector2Int pos, Transform target)
        {
            if(IsItValidCellPos(pos) == false)
            {
                return false; 
            }

            _gridController.PlacePieceAt(new Vector3Int(pos.x, pos.y, 0), target);
            return true;
        }

        public void RequestPieceUpdate(PieceUpdateArgs args)
        {
            _commandQueue.Enqueue(args);
        }

        private bool PlacePieceAt(Vector2Int pos, GameObject piece)
        {
            if (piece is null || CanPlacePiece(pos) == false)
            {
                return false;
            }

            _gridController.PlacePieceAt(new Vector3Int(pos.x, pos.y, 0), piece.transform);
            piece.transform.SetParent(_grid.transform);
            _datas[pos.x, pos.y].nowHoldingPiece = piece;

            CellUpdateEvent?.Invoke(new CellUpdateEventArgs(new Vector3Int(pos.x,pos.y,0), false, true));

            return true;
        }

        private bool UnplacePieceAt(Vector2Int pos)
        {
            if (IsItLocked(pos)) // IsItLocked에서 IsItValidCellPos이미 검사중임
            {
                return false;
            }

            if (_datas[pos.x, pos.y].nowHoldingPiece is null)
            {
                return false;
            }

            _datas[pos.x, pos.y].nowHoldingPiece.transform.SetParent(null);
            _datas[pos.x, pos.y].nowHoldingPiece = null;

            CellUpdateEvent?.Invoke(new CellUpdateEventArgs(new Vector3Int(pos.x, pos.y, 0), true, false));

            return true;
        }
        
        public void IncreaseUnlockedAreaToRight(int widthIncreasementRate)
        {
            if (widthIncreasementRate < 0)
            {
                Debug.LogError("늘릴 양은 음수가 될 수 없습니다");
                return;
            }

            if (widthIncreasementRate + _lockedAreaStartIdx > (int)MapMaxCellCnt.MAX_WIDTH)
            {
                widthIncreasementRate = (int)MapMaxCellCnt.MAX_WIDTH - _lockedAreaStartIdx;
            }
            
            if(widthIncreasementRate == 0)
            {
                return;
            }

            for(int x = _lockedAreaStartIdx; x < _lockedAreaStartIdx+widthIncreasementRate; x++)
            {
                for(int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
                {
                    _datas[x, y].isLocked = false;
                }
            }

            _lockedAreaStartIdx += widthIncreasementRate;
            _gridController.SetLockedAreaShadowXPos(_lockedAreaStartIdx);
        }

        public Vector2Int GetLastSelectedPos()
        {
            return _lastSelectedPos;
        }

        public int GetUnlockedAreaWidth()
        {
            return _lockedAreaStartIdx;
        }

        public int GetLockedAreaWidth()
        {
            return (int)MapMaxCellCnt.MAX_WIDTH - _lockedAreaStartIdx;
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

            if(outPiece is null)
            {
                return false;
            }

            return true;
        }

        private void GridMouseUpCallback(Vector3 mouseScreenPos)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3Int mouseCellPos = _grid.WorldToCell(mouseWorldPos);
            Vector2Int mouseCellPosVec2 = new Vector2Int(mouseCellPos.x, mouseCellPos.y);
            if (IsItValidCellPos(mouseCellPosVec2) == false || IsItLocked(mouseCellPosVec2))
            {
                _gridController.SetHighlightAt(new Vector3Int(-100, -100, 0));
                _lastSelectedPos = new Vector2Int(-1, -1);
                return;
            }
            _gridController.SetHighlightAt(new Vector3Int(mouseCellPosVec2.x, mouseCellPosVec2.y,0));
            _lastSelectedPos = mouseCellPosVec2;
        }
    }

}