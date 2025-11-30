using System;
using UnityEngine;

namespace ComponentModule
{
    public class HPComponentModule : MonoBehaviour
    {
        private int _HP = 0;
        private int _maxHP = 0;

        public void InitHP(int maxHP)
        {
            _HP = maxHP; 
            _maxHP = maxHP;
        }

        public int GetMaxHP()
        {
            return _maxHP; 
        }

        public int GetHP()
        {
            return _HP; 
        }

        public void TakeDamage(int damage)
        {
            if(damage < 0)
            {
                return;
            }

            _HP -= damage;

            if(_HP < 0)
            {
                _HP = 0;
            }
        }
    }
}