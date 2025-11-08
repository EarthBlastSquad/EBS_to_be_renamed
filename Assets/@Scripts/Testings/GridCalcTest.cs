using Contents.Grid;
using Controller;
using InputHandler;
using Manager.Contents;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils.Defines;

public class GridCalcTest : MonoBehaviour
{
    public Tilemap map;
    public GridManager mgr;
    public GridController c;
    public Grid grid;

    void Awake()
    {
        c = FindAnyObjectByType(typeof(GridController)) as GridController;
        mgr = FindAnyObjectByType(typeof(GridManager)) as GridManager;
        grid = FindAnyObjectByType(typeof(Grid)) as Grid;
        map = FindAnyObjectByType(typeof(Tilemap)) as Tilemap;
        Manager.Managers.Instance.ResourceManager.LoadAsyncAllIn("Test", cb);


    }

    [ContextMenu("f")]
    void Start()
    {
        //var h = FindFirstObjectByType(typeof(GridInputHandler)) as GridInputHandler;
        var h = FindFirstObjectByType<GridInputHandler>();
        //Debug.Log(h is null);
        h.mouseUpSubscriberEvent += Func;
        //findanyobjectbytype을 쓰면 콜백 등록이 2번쨰 실행부터 되던 버그
        //콜백 등록을 했는데, 왜 첫 플레이때는 안되고, 두번째 실행부터 되지?
    }

    private void cb(string _, int to, int end)
    {
        if(to == end)
        {
            mgr.Init();
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)&&false)
        {
            Debug.Log(grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }


    public void Func(Vector3 v)
    {
        Debug.Log(mgr.GetLastSelectedPos());
    }
    //gridmanager의 init은 GameScene에서 호출하는걸로
    //리소스 로딩은 어떻게 할거냐? ==> 로딩씬 만들고, 거기서 로드 끝난 후 실제 씬으로 넘기는 방식이나, 콜백으로 할듯?
    //난 개인적으로 전자가 좋다고 생각함
}
