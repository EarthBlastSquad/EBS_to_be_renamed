
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Manager.Core
{
    //string -> enum으로 방식 바꾸기
    public class ResourceManager
    {
        private Dictionary<int, ValueTuple<int,UnityEngine.Object>> _resources
            = new Dictionary<int, ValueTuple<int, UnityEngine.Object>>();//리소스 키의 해싱값 / <참조카운트,리소스>
        private Dictionary<string, List<int>> _lableStatus
            = new Dictionary<string, List<int>>();//라벨 / 리스트(리소스 키의 해싱값 <= 자기가 로드한 리소스 키들)

        public T Load<T>(string key) where T : UnityEngine.Object
        {


            if (_resources.TryGetValue(key.GetHashCode(), out var value))
            {
                return value.Item2 as T;
            }

#if UNITY_EDITOR
            Debug.LogWarning("load failed");
#endif

            return null;
        }

        //저장했던 프리팹 중 하나 꺼내서 인스턴스화 해서 리턴
        public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
        {
            GameObject prefab = Load<GameObject>($"{key}");
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab : {key}");
                return null;
            }

            if (pooling)
            {
                return Managers.Instance.ObjectPoolManager.GetFromPool(prefab);
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab, parent);

            go.name = prefab.name;
            return go;
        }

        public void Destroy(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (Managers.Instance.ObjectPoolManager.ReturnToPool(go))
            {
                return;
            }

            UnityEngine.Object.Destroy(go);
        }

        public void ReleaseIn(string lable)
        {
            List<int> keys;
            if(_lableStatus.TryGetValue(lable,out keys) == false)
            {
                return;
            }

            for(int i = 0;  i < keys.Count; i++)
            {
                var resource = _resources[keys[i]];
                Addressables.Release(resource.Item2);
                resource.Item1--;

                if(resource.Item1<=0)
                {
                    _resources.Remove(keys[i]);
                }
                else
                {
                    _resources[keys[i]] = resource;
                }
            }

            _lableStatus.Remove(lable);
        }

        public void ReleaseAll()
        {
            foreach(var keys in _lableStatus.Values)
            {
                for(int i = 0; i < keys.Count; i++)
                {
                    Addressables.Release(_resources[keys[i]].Item2);
                }
            }

            _resources.Clear();
            _lableStatus.Clear();
        }

        private void LoadAsync<T>(string lable, string key, Action<T> callback = null) where T : UnityEngine.Object
        {

            var asyncOperation = Addressables.LoadAssetAsync<T>(key);
            asyncOperation.Completed += (op) =>
            {
                Debug.Log($"resource : {op.Result.name}");

                int hashCode = key.GetHashCode();
                if(_resources.ContainsKey(hashCode) == false)
                {
                    _resources[hashCode] = (0,op.Result);
                }

                var resource = _resources[hashCode];
                resource.Item1++;
                _resources[hashCode] = resource;

                callback?.Invoke(op.Result);
            };
        }

        public void LoadAsyncAllIn(string lable, Action<string, int, int> callback)
        {
            if(_lableStatus.ContainsKey(lable))
            {
                callback?.Invoke(lable,1,1);
                return;
            }

            var asyncHandle = Addressables.LoadResourceLocationsAsync(lable, typeof(UnityEngine.Object));
            _lableStatus.Add(lable, new List<int>(16));

            asyncHandle.Completed += (handle) =>
            {
                int loadedCnt = 0;
                int targetCnt = handle.Result.Count;

                foreach (var result in handle.Result)
                {
                    Debug.Log($"path : {result.InternalId}\n{result.PrimaryKey}\n{result.InternalId}");

                    LoadAsync<UnityEngine.Object>(lable, result.PrimaryKey, (obj) =>
                    {
                        
                        loadedCnt++;
                        callback?.Invoke(result.PrimaryKey, loadedCnt, targetCnt);

                        if(loadedCnt == targetCnt)
                        {
                            Addressables.Release(asyncHandle);
                        }

                    });
                }
            };
        }
    }
}

//이름만 먼저 dict에 등록하고, 한번에 리소스 로드 후, 그걸 이름에 등록하는 형태로 가면 좋을거같은데 
/*
리소스 매니저에서 라벨단위로 내리면, 다른곳에서도 적용시켜야 하는데, 그거 어떻게 하지
근데, 씬 전환시에만 로드/언로드 할거같은데, 그러면 런타임에 되는게 아니니까 상관 없거든?
근데, 나중에 한번 손보긴 해야될듯
*/
/*
        타입단위로 로드 가능하니까, 그거에 맞춰서 로드 얼마나 됐는지 처리하기
        경로 로드해오고, 라벨단위로 로드해온 다음, 두개 매핑시키기
        경로 로드해오는 과정에서 실제 이름도 알 수 있다
        */
/*

라벨단위로 메모리에 올리고 내려야 한다

*/

// if (_resources.TryGetValue(key, out UnityEngine.Object resource))
// {
//     return resource as T;
// }

// //스프라이트 로드할때 항상 .sprite가 붙어 있어야하는데 데이터시트에 .sprite가 붙어있지 않은 데이터가 많음
// //임시로 붙임 -드래곤
// if (typeof(T) == typeof(Sprite))
// {
//     key = key + ".sprite";
//     if (_resources.TryGetValue(key, out Object temp))
//     {
//         return temp as T;
//     }
// }