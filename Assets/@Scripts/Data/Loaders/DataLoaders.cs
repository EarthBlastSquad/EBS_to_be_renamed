using System;
using System.Collections.Generic;
using Data;

[Serializable]
public class TowerDataLoader : ILoader<int, TowerData>
{
    public List<TowerData> towers = new List<TowerData>();

    public Dictionary<int, TowerData> MakeDict()
    {
        Dictionary<int, TowerData> dict = new Dictionary<int, TowerData>();
        foreach (TowerData data in towers)
        {
            dict.Add(data.TowerId, data);
        }
        return dict;
    }
}


