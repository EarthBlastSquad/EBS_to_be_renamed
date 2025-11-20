using System.Collections.Generic;
using Data;
using UnityEngine;
using Newtonsoft.Json;

namespace Manager.Core
{
    public class DataManager
    {
       public Dictionary<int, TowerData> TowerDic { get; private set; } = new Dictionary<int, TowerData>(); //예시:Managers.Instance.DataManager.TowerDic[1].TowerName






        //타이틀 씬에서 초기화 하는 구조던데 교보재는
        //이제 데이터 클래스 만들고, 그거 dict들 저장해야지
        public void Init()
        {
            TowerDic = LoadJson<TowerDataLoader, int, TowerData>("TowerData").MakeDict(); //<???DataLoader, int, ???Data>("Json 경로")





        }
        
        //솔직히, 이게 어떻게 가능한건지 아직 모르겠다
        TLoader LoadJson<TLoader,TKey,TVal>(string jsonPath) where TLoader : ILoader<TKey, TVal>
        {
            TextAsset textAsset = Managers.Instance.ResourceManager.Load<TextAsset>($"{jsonPath}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }

    }
}