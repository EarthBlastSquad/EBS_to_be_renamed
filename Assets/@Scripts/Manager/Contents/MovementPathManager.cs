using UnityEngine;
using Utils.Defines;
using System.Collections.Generic;
using System.Collections;
using Contents.Grid;
using DataStructure;

namespace Manager.Contents
{
    public class MovementPathManager : MonoBehaviour
    {
        //저장하는 데이터는 그 셀의 이전 셀의 좌표
        private Vector2Int[,] _calculatedPath = new Vector2Int[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];//접근할 때  x,y 형태로 접근할 것
        private GridManager _gridManager = null;
        private List<Vector2Int> _startPos = new List<Vector2Int>(4);
        private bool _dirty = false;
        private PriorityQueue _queue = new PriorityQueue();
        private int _steppableLayer = 0;

        private void Start()
        {
            _gridManager = GetComponent<GridManager>();
            if(_gridManager == null)
            {
                Debug.LogError("그리드 매니저가 이 오브젝트에 없음");
            }
            _steppableLayer = LayerMask.NameToLayer("SteppablePiece");
        }

        public Vector2Int GetNextPos(Vector2Int pos)
        {
            if(_gridManager.IsItValidCellPos(pos) == false)
            {
                return new Vector2Int((int)ControlValue.INVALID, (int)ControlValue.INVALID);
            }
            return _calculatedPath[pos.x, pos.y];
        }

        public bool CanMoveTo(Vector2Int pos)
        {
            GameObject piece;

            if(_gridManager.IsItValidCellPos(pos) == false)
            {
                return false;
            }

            if(_gridManager.TryGetPlacedPiece(pos,out piece) == false)
            {
                return true; 
            }

            return piece.layer == _steppableLayer;
        }

    }
}