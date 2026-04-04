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
            dict.Add(data.WaveIdx, data);
        }
        return dict;
    }
}


[Serializable]
public class AreaUnlockDataLoader : ILoader<int, AreaUnlockData>
{
    public List<AreaUnlockData> areaunlocks = new List<AreaUnlockData>();

    public Dictionary<int, AreaUnlockData> MakeDict()
    {
        Dictionary<int, AreaUnlockData> dict = new Dictionary<int, AreaUnlockData>();
        foreach (AreaUnlockData data in areaunlocks)
        {
            dict.Add(data.IDX, data);
        }
        return dict;
    }
}


[Serializable]
public class StageDataLoader : ILoader<int, StageData>
{
    public List<StageData> stages = new List<StageData>();

    public Dictionary<int, StageData> MakeDict()
    {
        Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
        foreach (StageData data in stages)
        {
            dict.Add(data.StageIdx, data);
        }
        return dict;
    }
}


[Serializable]
public class EndingDataLoader : ILoader<int, EndingData>
{
    public List<EndingData> endings = new List<EndingData>();

    public Dictionary<int, EndingData> MakeDict()
    {
        Dictionary<int, EndingData> dict = new Dictionary<int, EndingData>();
        foreach (EndingData data in endings)
        {
            dict.Add(data.EndingIdx, data);
        }
        return dict;
    }
}


[Serializable]
public class SkillDataLoader : ILoader<int, SkillData>
{
    public List<SkillData> skills = new List<SkillData>();

    public Dictionary<int, SkillData> MakeDict()
    {
        Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
        foreach (SkillData data in skills)
        {
            dict.Add(data.SkillID, data);
        }
        return dict;
    }
}


[Serializable]
public class TutorialDataLoader : ILoader<int, TutorialData>
{
    public List<TutorialData> tutorials = new List<TutorialData>();

    public Dictionary<int, TutorialData> MakeDict()
    {
        Dictionary<int, TutorialData> dict = new Dictionary<int, TutorialData>();
        foreach (TutorialData data in tutorials)
        {
            dict.Add(data.DataIdx, data);
        }
        return dict;
    }
}


