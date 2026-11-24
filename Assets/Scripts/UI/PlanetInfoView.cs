using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SolarSystem.Configs;

namespace SolarSystem.UI
{
    /// <summary>
    /// Presentation layer handling educational card popups on uGUI Canvas.
    /// Provides smooth responsive reveal and hide animations via DOTween.
    /// </summary>
    public sealed class PlanetInfoView : MonoBehaviour
    {
        [Header("UI Component Bindings")]
        [SerializeField] private RectTransform _panelRootRect;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _classificationText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Button _closeButton;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.35f;
        [SerializeField] private Vector2 _hiddenAnchoredPos = new Vector2(0f, -600f);
        [SerializeField] private Vector2 _visibleAnchoredPos = new Vector2(0f, 40f);

        private Tween _moveTween;
        private Tween _fadeTween;

        private void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(HideView);
            }

            // Start in hidden state
            if (_panelRootRect != null) _panelRootRect.anchoredPosition = _hiddenAnchoredPos;
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        public void DisplayPlanetInfo(PlanetDataSO data)
        {
            if (data == null) return;

            if (_titleText != null) _titleText.text = data.PlanetName;
            if (_classificationText != null) _classificationText.text = data.Classification;
            if (_descriptionText != null) _descriptionText.text = data.EducationalSummary;

            ShowView();
        }

        private void ShowView()
        {
            _moveTween?.Kill();
            _fadeTween?.Kill();

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
                _fadeTween = _canvasGroup.DOFade(1f, _animationDuration);
            }

            if (_panelRootRect != null)
            {
                _moveTween = _panelRootRect.DOAnchorPos(_visibleAnchoredPos, _animationDuration)
                    .SetEase(Ease.OutCubic);
            }
        }

        public void HideView()
        {
            _moveTween?.Kill();
            _fadeTween?.Kill();

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
                _fadeTween = _canvasGroup.DOFade(0f, _animationDuration * 0.7f);
            }

            if (_panelRootRect != null)
            {
                _moveTween = _panelRootRect.DOAnchorPos(_hiddenAnchoredPos, _animationDuration * 0.7f)
                    .SetEase(Ease.InCubic);
            }
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
            _fadeTween?.Kill();
            if (_closeButton != null) _closeButton.onClick.RemoveListener(HideView);
        }
    }
}
