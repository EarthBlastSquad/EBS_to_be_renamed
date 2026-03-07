using Coordinator;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class AttackManager : MonoBehaviour
    {
        private Queue<ValueTuple<BaseSkillCoordinator, VictimCoordinator, bool, int>> _queue = new Queue<(BaseSkillCoordinator, VictimCoordinator, bool, int)>(64);

        public void InitQueue()
        {
            _queue.Clear();
        }

        public void RequestAttack(ValueTuple<BaseSkillCoordinator, VictimCoordinator,bool,int> arg)
        {
            _queue.Enqueue(arg);
        }

        private void LateUpdate()
        {
            if(Managers.Instance.GameManager.IsGamePaused)
            {
                return; 
            }

            while(_queue.Count > 0)
            {
                var request = _queue.Dequeue();

                if(request.Item1 is null || request.Item2 is null)
                {
                    continue;
                }

                if(request.Item2.CanAttack())
                {
                    request.Item1.Act(request.Item2);//딱 공격만 하고, 체력 까임에 따른 이벤트는 각자가 알아서 처리할 것
                    Managers.Instance.GameManager.TotalDamages[(request.Item3, request.Item4)] += request.Item1.GetSkillDamage();
                }
            }
        }
    }
}