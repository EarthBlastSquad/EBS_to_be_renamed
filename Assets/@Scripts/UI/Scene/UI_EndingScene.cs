using DG.Tweening;
using Manager;
using Manager.Contents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.Defines;


namespace UI.Scene
{
    public class UI_EndingScene : UIScene
    {
        private TextMeshProUGUI _textBar;
        private EndingSceneManager _esm;

        private Tween _tween;
        private int _total;
        private string _currentText;
        private bool _isTotal=false;
        #region Enum
        enum GameObjects
        {
            Totals_0
        }

        enum Buttons
        {
            TextBar_0,
            TotalButton_0,
            SkipButton_0
        }

        enum Texts
        {

        }
        enum Images
        {
            Image_0
        }
        #endregion

        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            // 오브젝트 바인딩
            BindObject(typeof(GameObjects));
            BindButton(typeof(Buttons));
            BindText(typeof(Texts));
            BindImage(typeof(Images));
            _textBar = GetButton((int)Buttons.TextBar_0).GetComponentInChildren<TextMeshProUGUI>();
            _esm=FindAnyObjectByType<EndingSceneManager>();
            GetButton((int)Buttons.TextBar_0).gameObject.BindUIEvent(NextText);
            GetButton((int)Buttons.TotalButton_0).gameObject.BindUIEvent(TotalPopup);
            GetButton((int)Buttons.SkipButton_0).gameObject.BindUIEvent(SkipEnding);
            GetObject((int)GameObjects.Totals_0).SetActive(false);
            //GetContents();
            return true;
        }

        private void GetContents()
        {
            string s = "";
            switch (_esm.GetNowContent(ref s))
            {
                case EndingContentType.TYPE_IMAGE:
                    GetImage((int)Images.Image_0).sprite = Managers.Instance.ResourceManager.Load<Sprite>(s) as Sprite;
                    GetContents();
                    break;
                case EndingContentType.TYPE_TEXT:
                    _textBar.text = s;
                    _textBar.maxVisibleCharacters = 0;
                    _total = s.Length;
                    _tween?.Kill();
                    _tween = DOTween.To(() => _textBar.maxVisibleCharacters,x => _textBar.maxVisibleCharacters = x,_total, _total * 0.05f).SetEase(Ease.Linear); //함수화 해둘 이유가 없는거라 람다씀
                    break;
                case EndingContentType.TYPE_SOUND:
                    Managers.Instance.SoundManager.Play(0, s, true, Managers.Instance.GameManager.SoundValue);
                    GetContents();
                    break;
                case EndingContentType.TYPE_INVALID:
                    Managers.Instance.ResourceManager.LoadAsyncAllIn("LobbySceneLoaded", (key, count, totalCount) =>
                    {
                        if(count == totalCount)
                        {
                            Managers.Instance.SceneManagerEx.LoadScene(SceneNames.LobbyScene);
                            Managers.Instance.ResourceManager.ReleaseIn("EndingSceneLoaded");
                        }
                    });
                    return;
            }
        }
        #region 팝업

        #endregion
        #region 바인드용

        private void SkipEnding(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            string buffer="";
            while (_esm.GetNowContent(ref buffer) != EndingContentType.TYPE_INVALID) { }
            GetContents();
        }

        protected void NextText(PointerEventData _)
        {
            Managers.Instance.SoundManager.Play(SoundChannels.EFFECT_0, "ButtonPress", false);
            if (_tween != null && _tween.IsPlaying())
            {
                _tween.Kill();
                _textBar.maxVisibleCharacters = _total;
                return;
            }
            GetContents();
        }
        protected void TotalPopup(PointerEventData _)
        {
            _isTotal = !_isTotal;
            GetObject((int)GameObjects.Totals_0).SetActive(_isTotal);
        }
        #endregion
        private void Awake()
        {
            Init();
        }
        private void Start()
        {

        }
    }
}
