using System;
using UnityEngine;
using DG.Tweening;
using SolarSystem.Configs;

namespace SolarSystem.Simulation
{
    /// <summary>
    /// Controls procedural orbital trajectory around the Sun, self-rotation on axial tilt,
    /// and interactive camera focus transitions using DOTween.
    /// Optimized for mobile AR with zero GC allocations in Update.
    /// </summary>
    public sealed class OrbitalMotionController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PlanetDataSO _planetData;
        [SerializeField] private Transform _orbitCenter;
        [SerializeField] private Transform _planetModelTransform;

        public event Action<PlanetDataSO> OnPlanetSelected;

        private float _currentAngle;
        private Vector3 _cachedPosition;
        private Tween _focusScaleTween;

        public PlanetDataSO PlanetData => _planetData;

        private void Start()
        {
            ApplyInitialTransform();
        }

        private void ApplyInitialTransform()
        {
            if (_planetData == null) return;

            // Apply axial tilt to the planet mesh
            if (_planetModelTransform != null)
            {
                _planetModelTransform.localRotation = Quaternion.Euler(_planetData.AxialTiltAngle, 0f, 0f);
                _planetModelTransform.localScale = Vector3.one * _planetData.RelativeScale;
            }

            // Distribute initial orbital angle deterministically
            _currentAngle = UnityEngine.Random.Range(0f, 360f);
            UpdateOrbitPosition();
        }

        private void Update()
        {
            if (_planetData == null || _orbitCenter == null) return;

            // Orbital revolution around central star (Sun)
            _currentAngle += _planetData.OrbitalSpeed * Time.deltaTime;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            UpdateOrbitPosition();

            // Self-rotation along axial tilt
            if (_planetModelTransform != null)
            {
                _planetModelTransform.Rotate(Vector3.up, _planetData.RotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        private void UpdateOrbitPosition()
        {
            float radians = _currentAngle * Mathf.Deg2Rad;
            float scaledRadius = _planetData.OrbitRadius * _orbitCenter.lossyScale.x;
            float x = Mathf.Cos(radians) * scaledRadius;
            float z = Mathf.Sin(radians) * scaledRadius;

            _cachedPosition.x = _orbitCenter.position.x + x;
            _cachedPosition.y = _orbitCenter.position.y;
            _cachedPosition.z = _orbitCenter.position.z + z;

            transform.position = _cachedPosition;
        }

        /// <summary>
        /// Invoked when user taps on the planet collider in AR view.
        /// </summary>
        public void SelectPlanet()
        {
            _focusScaleTween?.Kill();

            if (_planetModelTransform != null)
            {
                float baseScale = _planetData.RelativeScale;
                _focusScaleTween = _planetModelTransform.DOScale(baseScale * 1.35f, 0.3f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad);
            }

            OnPlanetSelected?.Invoke(_planetData);
        }

        private void OnDestroy()
        {
            _focusScaleTween?.Kill();
        }
    }
}
