using Manager;
using UnityEngine;
namespace UI.Popup.Cell
{
    public class ItemCell : MonoBehaviour
    {
        public Contents.Tower.Tower ICTower { get; private set; }
        public void TowerSet(Contents.Tower.Tower t)
        {
            ICTower = t;
        }
    }
}

