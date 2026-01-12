using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public sealed class CameraShake2D : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Coroutine _shakeRoutine;

    void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float time = 0f;

        while (time < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
        _shakeRoutine = null;
    }
}
