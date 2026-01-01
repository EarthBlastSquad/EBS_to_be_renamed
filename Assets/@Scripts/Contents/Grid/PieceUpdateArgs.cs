using System;
using UnityEngine;
using Utils.Defines;

namespace Contents.Grid
{
    public struct PieceUpdateArgs
    {
        public PieceCommandTypes command;
        public Vector2Int pos;
        public Action<bool> commandStatusCallback;
        public GameObject instance;
    }
}