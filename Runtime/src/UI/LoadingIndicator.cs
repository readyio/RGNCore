using UnityEngine;
using UnityEngine.UI;

namespace RGN.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class LoadingIndicator : MonoBehaviour
    {
        [Header("_rotation")]
        [SerializeField] private bool _rotation = true;
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        [SerializeField] private float _rotationSpeed = 1;
        [SerializeField] private AnimationCurve _rotationAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("_rainbow")]
        [SerializeField] private bool _rainbow = true;
        [Range(-10, 10), Tooltip("Value in Hz (revolutions per second).")]
        [SerializeField] private float _rainbowSpeed = 0.5f;
        [Range(0, 1)]
        [SerializeField] private float _rainbowSaturation = 1f;
        [SerializeField] private AnimationCurve _rainbowAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Options")]
        [SerializeField] private bool _randomPeriod = true;

        private Image _image;
        private float _period;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _period = _randomPeriod ? Random.Range(0f, 1f) : 0;
        }
        private void Update()
        {
            if (_rotation)
            {
                transform.localEulerAngles = new Vector3(
                    0,
                    0,
                    -360 * _rotationAnimationCurve.Evaluate((_rotationSpeed * Time.time + _period) % 1));
            }

            if (_rainbow)
            {
                _image.color = Color.HSVToRGB(
                    _rainbowAnimationCurve.Evaluate((_rainbowSpeed * Time.time + _period) % 1),
                    _rainbowSaturation,
                    1);
            }
        }

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}
