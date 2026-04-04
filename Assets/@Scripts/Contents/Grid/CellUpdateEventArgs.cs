using System;
using UnityEngine;

namespace Contents.Grid
{
    public class CellUpdateEventArgs : EventArgs
    {
        private readonly Vector3Int _updatedPos;
        private readonly bool _oldHoldingPiece;
        private readonly bool _newHoldingPiece;

        // Constructor
        public CellUpdateEventArgs(Vector3Int updatedPos, bool oldHoldingPiece, bool newHoldingPiece)
        {
            _updatedPos = updatedPos;
            _oldHoldingPiece = oldHoldingPiece;
            _newHoldingPiece = newHoldingPiece;
        }

        public Vector3Int UpdatedPos => _updatedPos;
        public bool OldHoldingPiece => _oldHoldingPiece;
        public bool NewHoldingPiece => _newHoldingPiece;
    }
}