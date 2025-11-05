using UnityEngine;
using UnityEngine.Tilemaps;

namespace Controller
{
    public class GridController : MonoBehaviour
    {
        [SerializeField]
        private Tilemap _tileMap;
        private void Awake()
        {
            _tileMap = GetComponent<Tilemap>();
            if (_tileMap is null)
            {
                Debug.LogError("tilemap not found in grid");
            }
        }


    }
}
//이번에는 딱 클릭하고 하이라이팅까지만
//그 이상은 아직 생각하지 말자
//너무 깊게 생각하면 오히려 시작도 못한다
//해상도도 이번주에 가서 물어볼 때 생각해