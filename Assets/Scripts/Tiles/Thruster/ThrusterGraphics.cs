using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterGraphics : MonoBehaviour
{
    public ThrusterFlameController Flame;
    public SpriteRenderer EmissionSpriteRenderer;

    [ColorUsage(true, true)]
    public Color MinEmission, MaxEmission;

    [Range(0f, 1f)]
    public float PowerLevel = 0.3f;

    private void Update()
    {
        Color c = Color.Lerp(MinEmission, MaxEmission, PowerLevel);
        EmissionSpriteRenderer.material.SetColor("_EmissionColor", c);
        Flame.Magnitude = Mathf.Lerp(0.1f, 4f, PowerLevel);
    }
}
