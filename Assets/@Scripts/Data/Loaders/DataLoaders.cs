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


[Serializable]
public class MonsterDataLoader : ILoader<int, MonsterData>
{
    public List<MonsterData> monsters = new List<MonsterData>();

    public Dictionary<int, MonsterData> MakeDict()
    {
        Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
        foreach (MonsterData data in monsters)
        {
            dict.Add(data.MonsterId, data);
        }
        return dict;
    }
}


[Serializable]
public class WaveDataLoader : ILoader<int, WaveData>
{
    public List<WaveData> waves = new List<WaveData>();

    public Dictionary<int, WaveData> MakeDict()
    {
        Dictionary<int, WaveData> dict = new Dictionary<int, WaveData>();
        foreach (WaveData data in waves)
        {
            dict.Add(data.WaveTimeLimit, data);
        }
        return dict;
    }
}


