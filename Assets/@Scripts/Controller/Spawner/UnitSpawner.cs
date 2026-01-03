using Data;
using Manager.Contents;
using Scenes;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;

namespace Controller.Spawner
{
    public class UnitSpawner : MonoBehaviour
    {
        private GameScene _gameScene;
        private UnitManager _unitMgr;
        private List<Vector2Int> _startPos = new List<Vector2Int>(8);
        private int _mobSpawnInterval;
        private List<int> _mobIDs = new List<int>(0);

        private void Awake()
        {
            _unitMgr = FindAnyObjectByType<UnitManager>();   
            if(_unitMgr is null)
            {
#if UNITY_EDITOR
                Debug.LogError("unit manager does not exist");
#endif
            }

            _gameScene = FindAnyObjectByType<GameScene>();

            if (_gameScene is null)
            {
#if UNITY_EDITOR
                Debug.LogError("game scene script does not exist");
#endif
            }
            
            _gameScene.OnWaveChanged += OnWaveChanged;

            for (int y = 0; y < (int)MapMaxCellCnt.MAX_HEIGHT; y++)
            {
                _startPos.Add(new Vector2Int(0,y));
            }
        }

        private void OnWaveChanged(WaveData waveData)
        {
            _mobIDs = waveData.MobIDs;
            _mobSpawnInterval = waveData.MobSpawnRate;
        }
    }
}