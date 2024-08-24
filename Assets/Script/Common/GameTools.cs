using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTools
{
    public static IEnumerator FadeCanvasGroup(CanvasGroup target, float duration, float targetAlpha, bool unscaled = true)
    {
        if (target == null)
            yield break;

        float currentAlpha = target.alpha;

        float t = 0f;
        while (t < 1.0f)
        {
            if (target == null)
                yield break;

            float newAlpha = Mathf.SmoothStep(currentAlpha, targetAlpha, t);
            target.alpha = newAlpha;

            if (unscaled)
            {
                t += Time.unscaledDeltaTime / duration;
            }
            else
            {
                t += Time.deltaTime / duration;
            }

            yield return null;
        }

        target.alpha = targetAlpha;
    }

}