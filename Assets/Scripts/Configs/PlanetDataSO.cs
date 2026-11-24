using UnityEngine;

namespace SolarSystem.Configs
{
    /// <summary>
    /// ScriptableObject defining physical, orbital, and encyclopedic properties of a celestial body.
    /// </summary>
    [CreateAssetMenu(fileName = "PlanetData", menuName = "SolarSystem/PlanetData")]
    public sealed class PlanetDataSO : ScriptableObject
    {
        [Header("Identity & Info")]
        [SerializeField] private string _planetName = "Марс";
        [SerializeField] private string _classification = "Планета земной группы";
        [SerializeField, TextArea(3, 6)] private string _educationalSummary;

        [Header("Orbital Mechanics")]
        [SerializeField, Range(0.5f, 20f)] private float _orbitRadius = 2.5f;
        [SerializeField, Range(0.1f, 50f)] private float _orbitalSpeed = 1.0f;
        [SerializeField, Range(0.1f, 100f)] private float _rotationSpeed = 10f;
        [SerializeField] private float _axialTiltAngle = 25.19f;

        [Header("Visual Properties")]
        [SerializeField, Range(0.05f, 3f)] private float _relativeScale = 0.53f;
        [SerializeField] private Color _orbitTrailColor = new Color(0.85f, 0.45f, 0.25f, 0.4f);

        public string PlanetName => _planetName;
        public string Classification => _classification;
        public string EducationalSummary => _educationalSummary;
        public float OrbitRadius => _orbitRadius;
        public float OrbitalSpeed => _orbitalSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float AxialTiltAngle => _axialTiltAngle;
        public float RelativeScale => _relativeScale;
        public Color OrbitTrailColor => _orbitTrailColor;
    }
}
