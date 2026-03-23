using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Defines;
namespace Scenes
{
    public class LobbyScene : BaseScene
    {
        [SerializeField]
        private InputActionReference _backAction;
        private float _lastBackTime = 0f;
        //protected override void Init()
        //{
        //    base.Init();

        //    SceneType = Define.Scene.TitleScene;
        //    //TitleUI
        //}
        private void OnEnable()
        {
            _backAction.action.Enable();
            _backAction.action.performed -= OnBack;
            _backAction.action.performed += OnBack;
        }

        private void OnDisable()
        {
            _backAction.action.performed -= OnBack;
            _backAction.action.Disable();
        }
        private void OnBack(InputAction.CallbackContext ctx)
        {
            if (Time.time - _lastBackTime < 2f)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
            }
            else
            {
                _lastBackTime = Time.time;
                CloseGame("한 번 더 누르면 종료됩니다");
            }
        }
        private void CloseGame(string message)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using (AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast"))
            {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
                        "makeText",
                        context,
                        message,
                        toastClass.GetStatic<int>("LENGTH_SHORT")
                    );

                    toast.Call("show");
                }));
            }
        }
#else
            Debug.Log($"[TOAST] {message}");
#endif
        }
    }
}
