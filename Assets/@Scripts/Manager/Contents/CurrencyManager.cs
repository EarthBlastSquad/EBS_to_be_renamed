using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.Contents
{
    public class CurrencyManager
    {
        private int _currency = 0;
        public event Action<int, int> OnCurrencyChangedEvent;

        public CurrencyManager()
        {
            Manager.Managers.Instance.SceneManagerEx.SubscribeSceneUnloadedEvent(OnSceneUnload);
        }

        private void OnSceneUnload(Scene scene)
        {
            if (scene.name == Utils.Defines.SceneNames.GameScene.ToString())
            {
                OnCurrencyChangedEvent = null;
            }
        }

        public void RestoreCurrency(int currency, Type callerType)
        {
            if (callerType != typeof(GameManager))
            {
                return;
            }

            _currency = currency;
            OnCurrencyChangedEvent?.Invoke(_currency, currency);
        }

        public int GetCurrency { get { return _currency; } }

        public bool CanAfford(int amountToUse)
        {
            if (amountToUse < 0)
            {
#if DEBUG
                Debug.LogError("사용할 재화는 음수가 될 수 없습니다");
#endif
                return false;
            }

            return amountToUse <= _currency;
        }

        public bool UseCurrency(int amountToUse)
        {
            if (CanAfford(amountToUse) == false)
            {
#if DEBUG
                Debug.LogError("입력한만큼 재화를 사용할 수 없습니다.");
#endif
                return false;
            }

            OnCurrencyChangedEvent?.Invoke(_currency, _currency - amountToUse);
            _currency -= amountToUse;
            return true;
        }

        public void AddCurrency(int amountToAdd)
        {
            OnCurrencyChangedEvent?.Invoke(_currency, _currency + amountToAdd);
            _currency += amountToAdd;
        }
    }
}