using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteFlash : MonoBehaviour
{
    public float flashDuration = 0.1f;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    private List<Color> originalColors = new List<Color>();

    void Awake()
    {
        // Collect all SpriteRenderers in this object and its children
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
        foreach (var r in renderers)
            originalColors.Add(r.color);
    }

    public void FlashRed()
    {
        StopAllCoroutines(); // Prevent overlap if hit repeatedly
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Turn all sprites red
        foreach (var r in renderers)
            r.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        // Restore original colors
        for (int i = 0; i < renderers.Count; i++)
            renderers[i].color = originalColors[i];
    }
}
