using Data;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class AreaUnlockManager : MonoBehaviour
    {
        private AreaUnlockData _data;
        private GridManager _gridMgr;
        private bool _doesReachedEnd = false;   

        private void Awake()
        {
            _gridMgr = FindAnyObjectByType<GridManager>();

            if(_gridMgr is null)
            {
#if UNITY_EDITOR
                Debug.LogError("그리드매니저 없음");
#endif
            }
        }

        public void Init(AreaUnlockData data)
        {
            _data = data;
            _doesReachedEnd = false;
        }

        public bool DoesReachedEnd()
        {
            return _doesReachedEnd;
        }

        public bool TryUnlock()
        {
            if(DoesReachedEnd() || Managers.Instance.CurrencyManager.CanAfford(_data.DemendedCurrency) == false)
            {
                return false;
            }

            Managers.Instance.CurrencyManager.UseCurrency(_data.DemendedCurrency);

            _gridMgr.IncreaseUnlockedAreaToRight(_data.UnlockXSize);

            if (_data.NextUnlockData == (int)ControlValue.INVALID)
            {
                _doesReachedEnd = true;
                return true;
            }

            _data = Managers.Instance.DataManager.AreaUnlockDic[_data.NextUnlockData];

            return true;
        }

    }
}