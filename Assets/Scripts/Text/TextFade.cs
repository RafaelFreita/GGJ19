using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFade : MonoBehaviour
{

    public float time;
    public float distanceMultiplier;
    public AnimationCurve tween;

    private float _originalTime;

    public void Start()
    {
        _originalTime = time;
    }

    public void FadeOut(bool run)
    {
        if (run)
        {
            StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade()
    {
        while(time > 0)
        {
            transform.Translate(new Vector3(0, tween.Evaluate(time/_originalTime) * distanceMultiplier, 0));
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

}
