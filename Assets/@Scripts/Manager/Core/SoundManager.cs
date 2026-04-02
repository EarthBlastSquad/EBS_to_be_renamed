

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
namespace Manager.Core
{
    public class SoundManager
    {
        private AudioSource[] _audioSources = new AudioSource[(int)Utils.Defines.SoundChannels.MAX_CHANNELS];

        private Dictionary<string, AudioClip> _cachedAudioClips = new Dictionary<string, AudioClip>();

        private GameObject _soundRoot = null;


        public void Init()
        {
            if (_soundRoot == null)
            {
                _soundRoot = GameObject.Find("@SoundRoot");
                if (_soundRoot == null)
                {
                    _soundRoot = new GameObject { name = "@SoundRoot" };
                    UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

                    string[] soundTypeNames = System.Enum.GetNames(typeof(Utils.Defines.SoundChannels));
                    for (int count = 0, maxCnt = (int)Utils.Defines.SoundChannels.MAX_CHANNELS; count < maxCnt; count++)
                    {
                        GameObject go = new GameObject { name = soundTypeNames[count] };
                        _audioSources[count] = go.AddComponent<AudioSource>();
                        go.transform.parent = _soundRoot.transform;
                    }
                }
            }
        }

        public void Play(Utils.Defines.SoundChannels channel, string key, bool loop, float? volume = null, float pitch = 1.0f)
        {
            if(Managers.Instance.GameManager.SoundSet == false)
            {
                return;
            }

            if (channel < Utils.Defines.SoundChannels.BGM_0 || channel >= Utils.Defines.SoundChannels.MAX_CHANNELS)
            {
#if UNITY_EDITOR
                Debug.LogWarning("wrong channel number");
#endif  
                return;
            }
            float finalVolume = volume ?? Managers.Instance.GameManager.SoundValue;
            AudioSource audioSource = _audioSources[(int)channel];
            AudioClip clip = LoadAudioClip(key);

            if (clip is null)
            {
                return;
            }

            audioSource.pitch = pitch;
            audioSource.volume = finalVolume;

            if (channel < Utils.Defines.SoundChannels.EFFECT_0)
            {
                audioSource.Stop();
                audioSource.loop = loop;
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("eff");
#endif
                audioSource.PlayOneShot(clip, 1 - Mathf.Exp(-5 * finalVolume));
            }
        }

        public void PlayBGMInIdleChannel(Utils.Defines.SoundChannelTypes type, string key, bool loop, float? volume = null, float pitch = 1.0f)
        {
            Utils.Defines.SoundChannels channelName = Utils.Defines.SoundChannels.UNKNOWN;
            int channelCnt = GetChannelCnt(type);

            int channelIdxStart = (int)type;
            for (int channelIdxMax = channelCnt + channelIdxStart, i = channelIdxStart; i < channelIdxMax; i++)
            {
                if (_audioSources[i].isPlaying == false)
                {
                    channelName = (Utils.Defines.SoundChannels)i;
                    break;
                }
            }
            float finalVolume = volume ?? Managers.Instance.GameManager.SoundValue;
            Play(channelName, key, loop, finalVolume, pitch);
        }

        public void Stop(Utils.Defines.SoundChannels channelName)
        {
            if (channelName >= Utils.Defines.SoundChannels.BGM_0 && channelName < Utils.Defines.SoundChannels.MAX_CHANNELS)
            {
                _audioSources[(int)channelName].Stop();
            }
        }

        public void StopAll()
        {
            foreach (var source in _audioSources)
            {
                source.Stop();
            }
        }

        public void StopAllOf(Utils.Defines.SoundChannelTypes type)
        {
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for (int channelIdxMax = channelCnt + channelIdxStart, i = channelIdxStart; i < channelIdxMax; i++)
            {
                _audioSources[i].Stop();
            }
        }

        public void Clear()
        {
            StopAll();
            _cachedAudioClips.Clear();
        }


        private AudioClip LoadAudioClip(string audioClipName)
        {
            AudioClip clip;
            if (_cachedAudioClips.TryGetValue(audioClipName, out clip))
            {
                return clip;
            }

            clip = Managers.Instance.ResourceManager.Load<AudioClip>(audioClipName);
            if (clip is not null)
            {
                _cachedAudioClips.Add(audioClipName, clip);
            }
            return clip;
        }

        public int GetChannelCnt(Utils.Defines.SoundChannelTypes type)
        {
            switch (type)
            {
                case Utils.Defines.SoundChannelTypes.BGM:
                    return (int)Utils.Defines.SoundChannelCounts.BGM_CNT;
                case Utils.Defines.SoundChannelTypes.SUBBGM:
                    return (int)Utils.Defines.SoundChannelCounts.SUBBGM_CNT;
                case Utils.Defines.SoundChannelTypes.EFFECT:
                    return (int)Utils.Defines.SoundChannelCounts.EFFECT_CNT;
            }
            return -1;
        }

        public void FadeType(Utils.Defines.SoundChannelTypes type, float target, float duration)
        {
            int channelIdxStart = (int)type;
            int channelCnt = GetChannelCnt(type);

            for (int i = channelIdxStart; i < channelIdxStart + channelCnt; i++)
            {
                var source = _audioSources[i];
                if (source is null)
                {
                    continue;
                }
                source.volume = 0f;
                source.DOKill();
                source.DOFade(target, duration);
            }
        }

        public void CrossFadeType(Utils.Defines.SoundChannelTypes from, Utils.Defines.SoundChannelTypes to, float duration)
        {
            int fromStart = (int)from;
            int fromCount = GetChannelCnt(from);

            int toStart = (int)to;
            int toCount = GetChannelCnt(to);

            for (int i = toStart; i < toStart + toCount; i++)
            {
                var source = _audioSources[i];
                if (!source.isPlaying)
                {
                    source.Play();
                }
                source.volume = 0f;
                source.DOKill();
                DOVirtual.DelayedCall(duration, () =>
                {
                    source.DOFade(Managers.Instance.GameManager.SoundValue, duration).SetEase(Ease.InOutSine);
                });
            }

            for (int i = fromStart; i < fromStart + fromCount; i++)
            {
                var source = _audioSources[i];
                source.DOKill();
                source.DOFade(0f, duration).SetEase(Ease.InOutSine);
            }
        }
    }
    
}