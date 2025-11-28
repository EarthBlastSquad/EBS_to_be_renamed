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

        public void ModifyHP(int deltaHP)
        {
            _hp += deltaHP;
        }
    }
}