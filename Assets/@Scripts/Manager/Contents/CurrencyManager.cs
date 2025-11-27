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
        }
    }
}