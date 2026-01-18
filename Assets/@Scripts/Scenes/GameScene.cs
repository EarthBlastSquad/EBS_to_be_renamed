using Data;
using Manager;
using Manager.Contents;
using System;
using UnityEngine;
using Utils.Defines;

namespace Scenes
{
    public class GameScene : BaseScene
    {

        public Action<WaveData> OnWaveChanged;
        public Action<GameEndType> OnGameEnd;

        private WaveManager _waveMgr;
        private GridManager _gridMgr;
        private UnitManager _unitMgr;

        private void SaveClearData(bool isCleared)
        {
            int stageIdx = Managers.Instance.StageManager.GetNowStageData().StageIdx;
            if (isCleared)
            {
                Managers.Instance.GameManager.SetClearData(stageIdx, (666775,true));
                return;
            }

            int reachedWave = _waveMgr.GetNowWaveData().WaveNumber;
            if(Managers.Instance.GameManager.TryGetClearData(Managers.Instance.StageManager.GetNowStageData().StageIdx, out var data))
            {
                if(data.Item1 >= reachedWave)
                {
                    return;
                }
            }

            Managers.Instance.GameManager.SetClearData(stageIdx, (reachedWave,false));
            
        }

        private void CheckWinCondition()
        {
            if(_waveMgr.DoesReachedEnd() && _unitMgr.GetNowUnitCnt() <= 0)
            {
                OnGameEnd?.Invoke(GameEndType.WIN);
                SaveClearData(true);
            }
        }

        private void CheckLoseCondition()
        {
            for(int i = 0; i < (int)MapMaxCellCnt.MAX_HEIGHT; i++)
            {
                if(_gridMgr.TryGetReadonlyVictimList(new Vector2Int(0,i), out var victimList) && victimList.Count > 1)
                {
                    OnGameEnd?.Invoke(GameEndType.LOSE);
                    SaveClearData(false);
                    return;
                }
            }
        }

        public void OnSecondEnd()
        {
            if(_waveMgr.TryGetNextWave() == false)
            {
                Managers.Instance.TimerManager.Cleanup();
                return;
            }
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd,_waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
            OnWaveChanged?.Invoke(_waveMgr.GetNowWaveData());
        }

        protected override void Init()
        {
            base.Init();
            SceneType = Utils.Defines.SceneNames.GameScene;
            _waveMgr = FindAnyObjectByType<WaveManager>();
            _gridMgr = FindAnyObjectByType<GridManager>();
            _unitMgr = FindAnyObjectByType<UnitManager>();

#if UNITY_EDITOR

            if(_waveMgr is null ||  _gridMgr is null || _unitMgr is null)
            {
                Debug.LogError("웨이브 매니저 또는 그리드 매니저 또는 유닛 매니저가 없음");
            }
#endif

            bool result = _waveMgr.Init(Managers.Instance.StageManager.GetNowStageData().WaveIdx);

#if UNITY_EDITOR
            if(result == false)
            {
                Debug.LogError("웨이브 매니저 초기화 실패");
            }
#endif
        }

        private void Start()
        {
            _unitMgr.OnUnitDeadEvent += CheckWinCondition;
            _gridMgr.OnMobMovementEvent += CheckLoseCondition;
            FindAnyObjectByType<AreaUnlockManager>().Init(Managers.Instance.DataManager.AreaUnlockDic[Managers.Instance.StageManager.GetNowStageData().AreaUnlockIdx]);
            _gridMgr.Init();
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd, _waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
            OnWaveChanged?.Invoke(_waveMgr.GetNowWaveData());
        }
    }
}