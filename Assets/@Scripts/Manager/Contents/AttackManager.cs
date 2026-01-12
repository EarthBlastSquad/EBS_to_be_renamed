using Coordinator;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class AttackManager : MonoBehaviour
    {
        private Queue<ValueTuple<BaseSkillCoordinator, VictimCoordinator, VictimType>> _queue = new Queue<(BaseSkillCoordinator, VictimCoordinator, VictimType)>(64);

        public void InitQueue()
        {
            _queue.Clear();
        }

    }
}