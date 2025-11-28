using System;
using UnityEngine;

namespace ComponentModule
{
    public class HPComponentModule : MonoBehaviour
    {
        private int _hp = 0;

        public void RestoreHP(int hp)
        {
            _hp = hp;
        }

        public int GetHP()
        {
            return _hp; 
        }

        public void TakeDamage(int damage)
        {
            if(damage < 0)
            {
                return;
            }

            _hp -= damage;

            if(_hp < 0)
            {
                _hp = 0;
            }
        }
    }
}