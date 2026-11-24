using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using DG.Tweening;

namespace SolarSystem.AR
{
    /// <summary>
    /// Handles horizontal surface detection, user raycast hit testing,
    /// and smooth placement/scaling of the Solar System model in real space.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARPlaneManager))]
    public sealed class ARPlanePlacementController : MonoBehaviour
    {
        [Header("Placement Setup")]
        [SerializeField] private GameObject _solarSystemPrefab;
        [SerializeField] private Camera _arCamera;
        [SerializeField] private float _defaultScale = 0.25f;

        [Header("Animation")]
        [SerializeField] private float _spawnAnimationDuration = 0.8f;
        [SerializeField] private Ease _spawnEase = Ease.OutBack;

        public event Action<GameObject> OnSolarSystemPlaced;

        private ARRaycastManager _raycastManager;
        private ARPlaneManager _planeManager;
        private GameObject _spawnedInstance;
        private readonly List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>(8);

        public bool IsPlaced => _spawnedInstance != null;
        public GameObject SpawnedInstance => _spawnedInstance;

        private void Awake()
        {
            _raycastManager = GetComponent<ARRaycastManager>();
            _planeManager = GetComponent<ARPlaneManager>();
        }

        private void Update()
        {
            if (IsPlaced) return;

            // Detect touch on mobile device
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    TryPlaceSolarSystem(touch.position);
                }
            }
#if UNITY_EDITOR
            // Mouse click support for editor simulation
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceSolarSystem(Input.mousePosition);
            }
#endif
        }

        /// <summary>
        /// Raycasts against detected AR planes to anchor the Solar System at the hit point.
        /// </summary>
        private void TryPlaceSolarSystem(Vector2 screenPosition)
        {
            if (_raycastManager.Raycast(screenPosition, _raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = _raycastHits[0].pose;
                SpawnAndAnimate(hitPose.position, hitPose.rotation);
            }
        }

        private void SpawnAndAnimate(Vector3 position, Quaternion rotation)
        {
            if (_solarSystemPrefab == null)
            {
                Debug.LogError("[ARPlanePlacementController] SolarSystemPrefab is not assigned!");
                return;
            }

            _spawnedInstance = Instantiate(_solarSystemPrefab, position, rotation);
            _spawnedInstance.transform.localScale = Vector3.zero;

            // Smooth scale-in animation via DOTween
            _spawnedInstance.transform.DOScale(Vector3.one * _defaultScale, _spawnAnimationDuration)
                .SetEase(_spawnEase)
                .OnComplete(() =>
                {
                    // Disable plane visualization once the model is placed to save GPU draw calls
                    DisablePlaneVisuals();
                    OnSolarSystemPlaced?.Invoke(_spawnedInstance);
                });
        }

        private void DisablePlaneVisuals()
        {
            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            _planeManager.enabled = false;
        }

        public void ResetPlacement()
        {
            if (_spawnedInstance != null)
            {
                Destroy(_spawnedInstance);
                _spawnedInstance = null;
            }

            _planeManager.enabled = true;
            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
        }
    }
}
