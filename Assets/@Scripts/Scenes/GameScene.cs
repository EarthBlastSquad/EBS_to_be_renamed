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

        public void OnSecondEnd()
        {
            if(_waveMgr.TryGetNextWave() == false)
            {
                return;
            }
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd,_waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
        }

        protected override void Init()
        {
            base.Init();
            SceneType = Utils.Defines.SceneNames.GameScene;
            _waveMgr = FindAnyObjectByType<WaveManager>();

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
            Managers.Instance.TimerManager.StartTimer(OnSecondEnd, _waveMgr.GetNowWaveData().WaveTimeLimit, 1f);
            OnWaveChanged?.Invoke(_waveMgr.GetNowWaveData());
        }
    }
}