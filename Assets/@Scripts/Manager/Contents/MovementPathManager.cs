using Contents.Grid;
using DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;
using static UnityEditor.PlayerSettings;

namespace Manager.Contents
{
    public class MovementPathManager : MonoBehaviour
    {
        //저장하는 데이터는 그 셀의 이전 셀의 좌표
        private Vector2Int[,] _calculatedPath = new Vector2Int[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];//접근할 때  x,y 형태로 접근할 것
        private GridManager _gridManager = null;
        private List<Vector2Int> _startPos = new List<Vector2Int>(8);
        private bool _dirty = false;
        private PriorityQueue _queue = new PriorityQueue();
        private int _steppableLayer = 0;
        private Vector2Int _invalidVector = new Vector2Int((int)ControlValue.INVALID, (int)ControlValue.INVALID);

        private void Start()
        {
            _gridManager = GetComponent<GridManager>();
            if(_gridManager == null)
            {
                Debug.LogError("그리드 매니저가 이 오브젝트에 없음");
            }
            _steppableLayer = LayerMask.NameToLayer("SteppablePiece");
            _gridManager.CellUpdateEvent += RequestUpdatePath;

            for (int i = 0; i < (int)MapMaxCellCnt.MAX_HEIGHT;i++)
            {
                _startPos.Add(new Vector2Int(0, i));
            }
        }

        private void RequestUpdatePath(CellUpdateEventArgs arg)
        {
            if(_dirty)
            {
                return; 
            }

            _dirty = true;
            StartCoroutine(CalculatePath());
        }

        private void EnqueueVert(int oldWeight, Vector2Int nowVert ,int nextX, int nextY)
        {
            Vector2Int nextVert = new Vector2Int(nextX, nextY);
            if(_gridManager.IsItValidCellPos(nextVert) == false)
            {
                return; 
            }

            if (_calculatedPath[nextX,nextY].x != (int)ControlValue.INVALID)
            {
                return;
            }

            int weight = (int)Weights.CAN_GO;

            if(_gridManager.TryGetPlacedPiece(nextVert,out GameObject outPiece) && outPiece.layer != _steppableLayer)
            {
                weight = (int)Weights.PIECE;
            }
            _queue.Enqueue(weight + oldWeight,nowVert, nextVert);
        }
        
        private IEnumerator CalculatePath()
        {
            yield return new WaitForEndOfFrame();
            _dirty = false;
            if(_startPos.Count <= 0)
            {
                yield break;
            }

            _queue.Clear();

            for(int x = 0; x < (int)MapMaxCellCnt.MAX_WIDTH; x++)
            {
                for(int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
                {
                    _calculatedPath[x, y] = _invalidVector;
                }
            }
            
            foreach(var start in _startPos)
            {
                _queue.Enqueue(0, new Vector2Int((int)ControlValue.START, (int)ControlValue.START), start);
            }

            while(!_queue.IsItEmpty())
            {
                ValueTuple<int,Vector2Int,Vector2Int> nowVert = _queue.Dequeue();

                if(nowVert.Item2.x == (int)ControlValue.INVALID || _calculatedPath[nowVert.Item3.x,nowVert.Item3.y].x != (int)ControlValue.INVALID)
                {
                    continue;
                }

                _calculatedPath[nowVert.Item3.x, nowVert.Item3.y] = nowVert.Item2;
                EnqueueVert(nowVert.Item1, nowVert.Item3, nowVert.Item3.x - 1  , nowVert.Item3.y);
                EnqueueVert(nowVert.Item1, nowVert.Item3, nowVert.Item3.x + 1  , nowVert.Item3.y);
                EnqueueVert(nowVert.Item1, nowVert.Item3, nowVert.Item3.x      , nowVert.Item3.y - 1);
                EnqueueVert(nowVert.Item1, nowVert.Item3, nowVert.Item3.x      , nowVert.Item3.y + 1);
            }
        }
        /*
         큐에서 뽑아온다
        갈 수 있는지 검사한다(이미 방문 했나)//이건 큐에 남아있는건 확정돼도 상태 전파가 안돼서 이중체킹 해야될듯
        경로를 확정시킨다
        갈 수 있는 방향을 큐에 넣어둔다(올바른 좌표인가, 이미 방문 했나 )
         */

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

        public bool CanMoveInAir(Vector2Int pos)
        {
            return _gridManager.IsItValidCellPos(pos) && _calculatedPath[pos.x, pos.y].x != (int)ControlValue.START && _calculatedPath[pos.x, pos.y].x != (int)ControlValue.INVALID;
        }

    }
}