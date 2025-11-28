using Manager.Contents;
using System;
using UnityEngine;

namespace ComponentModule
{
    public class HPComponentModule : MonoBehaviour
    {
        private int _hp = 0;

        public void RestoreHP(int hp, Type callerType)
        {
            if (callerType != typeof(GameManager))
            {
                return;
            }

            _hp = hp;
        }
    }
}