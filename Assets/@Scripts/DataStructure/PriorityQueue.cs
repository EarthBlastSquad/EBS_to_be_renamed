using System;
using Unity.Mathematics;
using UnityEngine;
using Utils.Defines;

namespace DataStructure
{
    public class PriorityQueue
    {
        private ValueTuple<int,Vector2Int>[] _heap = new ValueTuple<int, Vector2Int>[127];
        private int _maxRank = 7;
        private int _idx=0;
        
        public void Clear()
        {
            _idx = 0;
        }

        public bool IsItEmpty()
        {
            return _idx <= 0;
        }

        public void Enqueue(int weight, Vector2Int pos)
        {
            if(_idx >= _heap.Length)
            {
                _maxRank += 2;
                Array.Resize(ref _heap, (int)math.pow(2,_maxRank)-1);
            }

            _heap[_idx] = ValueTuple.Create(weight, pos);

            int parent = _idx;
            int child = _idx;

            while (parent > 0)
            {
                parent = (child - 1) / 2;
                if (_heap[parent].Item1 <= _heap[child].Item1)
                {
                    break;
                }
                var tmp = _heap[parent];
                _heap[parent] = _heap[child];
                _heap[child] = tmp;
                child = parent;
            }

            _idx++;
        }

        public ValueTuple<int,Vector2Int> Dequeue()
        {
            if(_idx <= 0)
            {
                Debug.LogError("empty heap");
                return new ValueTuple<int, Vector2Int>((int)ControlValue.INVALID,new Vector2Int((int)ControlValue.INVALID, (int)ControlValue.INVALID));
            }

            _idx--;
            var tmp = _heap[0];
            _heap[0] = _heap[_idx];

            int parent = 0;
            int left = 0;
            int right = 0;

            while (true)
            {
                left = parent * 2 + 1;
                right = parent * 2 + 2;

                if(left>=_idx)
                {
                    break;
                }

                int child = -1;

                int parentWeight = _heap[parent].Item1;
                int leftWeight = int.MaxValue;
                int rightWeight = int.MaxValue;

                if(left < _idx)
                {
                    leftWeight = _heap[left].Item1;
                }

                if(right < _idx)
                {
                    rightWeight = _heap[right].Item1;
                }

                if(parentWeight < rightWeight && parentWeight < leftWeight)
                {
                    break;
                }

                if(right < _idx && rightWeight<leftWeight)
                {
                    child = right;
                }
                else
                {
                    child = left;
                }

                var buf = _heap[parent];
                _heap[parent] = _heap[child];
                _heap[child] = buf;
                parent = child;
            }

            return tmp;
        }

    }
}