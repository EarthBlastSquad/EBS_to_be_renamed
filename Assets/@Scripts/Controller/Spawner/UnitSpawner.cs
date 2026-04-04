using Coordinator;
using Data;
using Manager;
using Manager.Contents;
using Scenes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils.Defines;

namespace Controller.Spawner
{
    public class UnitSpawner : MonoBehaviour
    {
        private GameScene _gameScene;
        private UnitManager _unitMgr;
        private List<Vector2Int> _startPos = new List<Vector2Int>(8);
        private float _mobSpawnInterval;
        private float _accumulatedTime = 0;
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
                _startPos.Add(new Vector2Int((int)MapMaxCellCnt.MAX_WIDTH - 1,y));
            }
        }

        private void Update()
        {
            if(Managers.Instance.GameManager.IsGamePaused || _mobIDs.Count <= 0)
            {
                return;
            }

            _accumulatedTime += Time.deltaTime;

            if(_accumulatedTime >= _mobSpawnInterval)
            {
                _accumulatedTime -= _mobSpawnInterval;

                MonsterData monsterData = Managers.Instance.DataManager.MonsterDic[_mobIDs[Random.Range(0, _mobIDs.Count)]];
                GameObject go = Managers.Instance.ResourceManager.Instantiate(monsterData.PrefabName, pooling: true);
                if(go is null)
                {
                    return;
                }
                UnitCoordinator coordinator = go.GetOrAddComponent<UnitCoordinator>();
                coordinator.Init(monsterData, _startPos[Random.Range(0,_startPos.Count)]);
                _unitMgr.AddUnit(coordinator);
            }
        }

        public void OnWaveChanged(WaveData waveData)
        {
            _accumulatedTime = waveData.MobSpawnRate;
            _mobIDs = waveData.MobIDs;
            _mobSpawnInterval = waveData.MobSpawnRate;
        }
    }
}