using System;
using UnityEngine;

namespace Manager.Contents
{
    public class CurrencyManager
    {
        private int _currency = 0;
        public event Action<int,int> OnCurrencyChangedEvent;

        public void RestoreCurrency(int currency, Type callerType)
        {
            if(callerType != typeof(GameManager))
            {
                return;
            }

            _currency = currency;
            OnCurrencyChangedEvent?.Invoke(_currency, currency);
        }

        public int GetCurrency { get { return _currency; } }

        public bool CanAfford(int amountToUse)
        {
            if(amountToUse < 0)
            {
#if DEBUG
                Debug.LogError("사용할 재화는 음수가 될 수 없습니다");
#endif
                return false;
            }

            return amountToUse <= _currency;
        }
    }
}