using Contents.Grid;
using DataStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class MovementPathManager : MonoBehaviour
    {
        private int[,] _weightMap = new int[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];
        //저장하는 데이터는 그 셀의 이전 셀의 좌표
        private Vector2Int[,] _calculatedPath = new Vector2Int[(int)MapMaxCellCnt.MAX_WIDTH, (int)MapMaxCellCnt.MAX_HEIGHT];//접근할 때  x,y 형태로 접근할 것
        private GridManager _gridManager = null;
        private List<Vector2Int> _startPos = new List<Vector2Int>(8);
        private bool _dirty = false;
        private PriorityQueue _queue = new PriorityQueue();
        private int _steppableLayer = 0;
        private Vector2Int _invalidVector = new Vector2Int((int)ControlValue.INVALID, (int)ControlValue.INVALID);
        public event Action OnPathRecalculated;

        private void Awake()
        {
            _gridManager = GetComponent<GridManager>();
            if (_gridManager == null)
            {
#if UNITY_EDITOR
                Debug.LogError("그리드 매니저가 이 오브젝트에 없음");
#endif
            }
            _steppableLayer = LayerMask.NameToLayer("SteppablePiece");
            _gridManager.CellUpdateEvent += RequestUpdatePath;

            for (int i = 0; i < (int)MapMaxCellCnt.MAX_HEIGHT; i++)
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

            //int weight = (int)Weights.CAN_GO;

            //if (_gridManager.TryGetPlacedPiece(nextVert, out GameObject outPiece) && outPiece.layer != _steppableLayer)
            //{
            //    weight = (int)Weights.PIECE;
            //}
            int weight = _weightMap[nextVert.x, nextVert.y];
            _queue.Enqueue(weight + oldWeight,nowVert, nextVert);
        }

        private void SetPieceNearWeight(int x, int y)
        {
            var pos = new Vector2Int(x, y);
            if(_gridManager.IsItValidCellPos(pos) && (_weightMap[x,y] != (int)Weights.PIECE))
            {
                _weightMap[pos.x, pos.y] = (int)Weights.PIECE_NEAR;
            }
        }

        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_weightMap == null) return;

            for (int x = 0; x < (int)MapMaxCellCnt.MAX_WIDTH; x++)
            {
                for (int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
                {
                    Vector3 worldPos = _gridManager.GetWorldPos(new Vector2Int(x, y), 0); // 그리드 좌표를 월드 좌표로 변환

                    // 가중치에 따라 색상 변경 (벽 근처는 푸른색, 일반은 흰색, 벽은 붉은색 등)
                    int w = _weightMap[x, y];
                    Gizmos.color = (w == (int)Weights.PIECE) ? Color.red :
                                   (w == (int)Weights.PIECE_NEAR) ? Color.cyan : Color.white;

                    Gizmos.DrawWireCube(worldPos, Vector3.one * 0.9f);

                    // 가중치 수치를 텍스트로 표시
                    Handles.Label(worldPos, w.ToString());
                }
            }

        }
#endif

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
                    _weightMap[x, y] = (int)ControlValue.INVALID;
                }
            }

            for (int x = 0; x < (int)MapMaxCellCnt.MAX_WIDTH; x++)
            {
                for (int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
                {
                    int w = _weightMap[x, y];
                    if (w == (int)Weights.PIECE)
                    {
                        continue;
                    }
                    Vector2Int pos = new Vector2Int(x,y);
                    if(_gridManager.TryGetPlacedPiece(pos,out var p) == false || p.layer == _steppableLayer)
                    {
                        if(w != (int)Weights.PIECE_NEAR)
                        {
                            _weightMap[x, y] = (int)Weights.CAN_GO;
                        }
                        continue;
                    }

                    _weightMap[x, y] = (int)Weights.PIECE;
                    SetPieceNearWeight(x-1,y);
                    SetPieceNearWeight(x+1,y);
                    SetPieceNearWeight(x ,y-1);
                    SetPieceNearWeight(x ,y+1);
                }
            }

            foreach (var start in _startPos)
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
            OnPathRecalculated?.Invoke();
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
        public int GetRemainingStep(Vector2Int pos)
        {
            int count;
            for (count = 0; count < (int)MapMaxCellCnt.MAX_HEIGHT * (int)MapMaxCellCnt.MAX_WIDTH; count++)
            {
                Vector2Int next = GetNextPos(pos);

                if (next.x == (int)ControlValue.START)
                {
                    break;
                }
                if (next.x == (int)ControlValue.INVALID)
                {
                    return int.MaxValue;
                }
                pos = next;
            }

            return count;
        }
    }
}