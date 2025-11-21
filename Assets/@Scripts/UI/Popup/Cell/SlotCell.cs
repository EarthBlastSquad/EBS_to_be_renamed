using System.Text.RegularExpressions;
using UnityEngine;

namespace UI.Popup.Cell
{
    public class SlotCell : MonoBehaviour
    {
        public sbyte SlotIndex {  get; private set; }
        public void Init()
        {
            SlotIndex=sbyte.Parse(Regex.Match(gameObject.name, @" ?_(\d+)$").Groups[1].Value);
#if UNITY_EDITOR
            Debug.Log(SlotIndex);
#endif
        }
    }
}