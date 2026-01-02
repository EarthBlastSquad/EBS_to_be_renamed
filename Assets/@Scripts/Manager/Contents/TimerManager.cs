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
                _onTimerEnd = null;
            }
        }
    }
}