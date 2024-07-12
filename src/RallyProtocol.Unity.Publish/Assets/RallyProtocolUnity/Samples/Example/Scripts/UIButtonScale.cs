using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonScale : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] public UIScaleTween uiScaleTween;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        if (uiScaleTween.transform != null)
        {
            uiScaleTween.transform.localScale = Vector3.zero;
            uiScaleTween.ScaleTransformOnStart();
        }


        uiScaleTween.transform.GetComponent<Button>().onClick.AddListener(OnScaleUpButtonPressed);
        
    }

    private void OnDestroy()
    {

        uiScaleTween.transform.GetComponent<Button>().onClick.RemoveListener(OnScaleUpButtonPressed);
     
    }
    #endregion

    #region Methods
    public void OnScaleUpButtonPressed()
    {
        if (uiScaleTween != null )
        {
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(ScaleUpAndDown());
        }
    }

    private IEnumerator ScaleUpAndDown()
    {
        yield return StartCoroutine(uiScaleTween.ScaleCoroutine(uiScaleTween.transform, uiScaleTween.targetScale, uiScaleTween.scaleDuration));
        yield return StartCoroutine(uiScaleTween.ScaleCoroutine(uiScaleTween.transform, uiScaleTween.initialScale, uiScaleTween.scaleDuration));
    }
    #endregion
}
