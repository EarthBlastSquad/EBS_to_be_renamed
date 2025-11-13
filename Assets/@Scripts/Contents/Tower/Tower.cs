using Manager;
using UnityEngine;
namespace Contents.Tower
{
    public class Tower
    {
        public int key=-1;
        public Data.TowerData TowerData;
        public bool IsOwned = false;
        public bool IsEquipped = false;
        public sbyte Slot = -1; //비효율적인 것 같기도 한데, 그냥 있는 구조 그대로 들고오는게 빠름
        public Tower() //역직렬화용 기본 생성자
        {

        }
        public Tower(int k=0)
        {
            if(k==0)
            {
#if UNITY_EDITOR
                Debug.Log("?");
#endif
                return;
            }
            key=k;
            TowerData = Managers.Instance.DataManager.TowerDic[key];
        }
    }

}
