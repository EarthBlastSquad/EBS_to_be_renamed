using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.Contents
{
    public class TimerManager : MonoBehaviour
    {
        //현재 시간 , 목표 시간
        private Action _onTimerEnd;
        public Action<float, float> OnSecondChanged;
        private float _time = 0;
        private float _accumulatedTime = 0;
        private float _totalTime = 0;
        private float _callInterval = 0;


        public void Cleanup()
        {
            OnSecondChanged = null;
            _onTimerEnd = null;
        }

        public void StartTimer(Action timerEndCallback, float time, float callInterval)
        {
            _onTimerEnd = timerEndCallback;
            _time = time;
            _callInterval = callInterval;
            _totalTime = 0;
            _accumulatedTime = 0;
            OnSecondChanged?.Invoke(_totalTime,_time);
        }

        private void Update()
        {
            if(_onTimerEnd is null || Managers.Instance.GameManager.IsGamePaused)
            { 
                return;
            }

            _accumulatedTime += Time.deltaTime;
            _totalTime += Time.deltaTime;

            if(_accumulatedTime >= _callInterval)
            {
                _accumulatedTime = 0;
                OnSecondChanged?.Invoke(_totalTime, _time);
            }

            if(_totalTime >= _time)
            {
                OnSecondChanged?.Invoke(_totalTime, _time);
                _onTimerEnd?.Invoke();
                //_onTimerEnd = null; //어차피 시작마다 콜백 줘야하는데, 굳이 여기서 초기화 할 필요 없음 <- 사용하는 장소 보면 더더욱. CleanUp도 있으니까, 이 부분은 굳이 있을 필요 없음
            }
        }
    }
}