using Data;
using Manager;
using Manager.Contents;
using System;
using UnityEngine;

namespace Scenes
{
    public class GameScene : BaseScene
    {

        public Action<WaveData> OnWaveChanged;

        private WaveManager _waveMgr;
        private GridManager _gridMgr;

        public void OnSecondEnd()
        {
            if(_waveMgr.TryGetNextWave() == false)
            {
                Managers.Instance.TimerManager.Cleanup();
                return;
            }
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd,_waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
        }

        protected override void Init()
        {
            base.Init();
            SceneType = Utils.Defines.SceneNames.GameScene;
            _waveMgr = FindAnyObjectByType<WaveManager>();
            _gridMgr = FindAnyObjectByType<GridManager>();

#if UNITY_EDITOR

            if(_waveMgr is null ||  _gridMgr is null)
            {
                Debug.LogError("웨이브 매니저 또는 그리드 매니저가 없음");
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
            FindAnyObjectByType<AreaUnlockManager>().Init(Managers.Instance.DataManager.AreaUnlockDic[Managers.Instance.StageManager.GetNowStageData().AreaUnlockIdx]);
            _gridMgr.Init();
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd, _waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
            OnWaveChanged?.Invoke(_waveMgr.GetNowWaveData());
        }
    }
}