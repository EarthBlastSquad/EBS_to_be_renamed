using Data;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class WaveManager : MonoBehaviour
    {
        private WaveData _data;

        public bool Init(int waveIdx)
        {
            if(Managers.Instance.DataManager.WaveDic.TryGetValue(waveIdx, out _data))
            {
                return true;
            }

            return false;
        }

        public bool DoesReachedEnd()
        {
            return _data.NextWaveIdx == (int)ControlValue.INVALID; 
        }

        public WaveData GetNowWaveData()
        {
            return _data; 
        }

        public bool TryGetNextWave()
        {
            if(DoesReachedEnd() || (Managers.Instance.DataManager.WaveDic.TryGetValue(_data.NextWaveIdx, out _data) == false))
            {
                return false;
            }

            return true;
        }

    }
}