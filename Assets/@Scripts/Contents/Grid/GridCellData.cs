using Coordinator;
using System.Collections.Generic;
using UnityEngine;

namespace Contents.Grid
{
    public struct GridCellData
    {
        public GameObject nowHoldingPiece;
        public List<VictimCoordinator> victimList;
        public int cellImgNumber;
        public bool isLocked;
    }
}