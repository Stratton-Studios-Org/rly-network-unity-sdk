using System.Collections;
using UnityEngine;

public class UIScaleTween : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Transform targetTransform;
    [SerializeField] public float scaleDuration = 1f;
    [SerializeField] public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] public Vector3 initialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    #endregion

 

    #region Methods

    /// <summary>
    /// Scales the transform to the target scale with overshoot.
    /// </summary>
    public void ScaleTransform()
    {
        if (targetTransform != null)
        {
            StartCoroutine(ScaleCoroutine(targetTransform, targetScale, scaleDuration));
        }
    }

    /// <summary>
    /// Scales the transform to the initial scale on start.
    /// </summary>
    public void ScaleTransformOnStart()
    {
        if (targetTransform != null)
        {
            StartCoroutine(ScaleCoroutine(targetTransform, initialScale, scaleDuration));
        }
    }

    /// <summary>
    /// Resets the transform scale to its initial size with overshoot.
    /// </summary>
    public void ResetScale()
    {
        if (targetTransform != null)
        {
            StartCoroutine(ScaleCoroutine(targetTransform, initialScale, scaleDuration));
        }
    }

    /// <summary>
    /// Coroutine to handle the scaling with overshoot over time.
    /// </summary>
    /// <param name="transform">The Transform to scale.</param>
    /// <param name="targetScale">The target scale.</param>
    /// <param name="duration">The duration of the scaling.</param>
    /// <returns></returns>
    public IEnumerator ScaleCoroutine(Transform transform, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float curveValue = scaleCurve.Evaluate(time / duration);
            transform.localScale = Vector3.LerpUnclamped(initialScale, targetScale, curveValue);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    #endregion
}
