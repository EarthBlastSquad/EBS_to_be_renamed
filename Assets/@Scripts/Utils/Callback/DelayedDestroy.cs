using Manager;
using System.Collections;
using UnityEngine;

namespace Utils.Callback
{
    public class DelayedDestroy : MonoBehaviour
    {
        public void DestroyAfter(float time)
        {
            StartCoroutine(InternalCoroutine(time));
        }

        protected IEnumerator InternalCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            Managers.Instance.ResourceManager.Destroy(gameObject);
        }
    }
}
