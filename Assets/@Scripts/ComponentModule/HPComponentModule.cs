using System;
using UnityEngine;

namespace ComponentModule
{
    public class HPComponentModule : MonoBehaviour
    {
        private int _hp = 0;
        private int _maxHP = 0;

        public void InitHP(int maxHP)
        {
            _hp = maxHP; 
            _maxHP = maxHP;
        }

        public int GetMaxHP()
        {
            return _maxHP; 
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