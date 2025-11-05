using Contents.Grid;
using Controller;
using UnityEngine;

namespace Manager.Contents
{
    public class GridManager : MonoBehaviour
    {
        private GridController _gridController;
        private Grid _grid;
        private GridCellData[,] _datas;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            if (_grid is null)
            {
                Debug.LogError("grid not found in grid");
            }

            _gridController = GetComponentInChildren<GridController>();
            if(_gridController is null)
            {
                Debug.LogError("grid controller not found in child");
            }
        }
    }

}
/*
딱 이미지 셋업까지만


*/