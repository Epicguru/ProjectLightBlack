using UnityEngine;

public class ThrusterFlameController : MonoBehaviour
{
    public SpriteRenderer Renderer;

    [Range(0f, 10f)]
    public float Magnitude = 0.5f;

    [Range(0.01f, 100f)]
    public float Frequency = 5f;
    [Range(0f, 2f)]
    public float Amplitude = 0.4f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        float wave = Mathf.Sin((timer * 2f * Mathf.PI) * Frequency) * Amplitude;
        float scale = Magnitude + wave;

        Vector3 ls = transform.localScale;
        ls.x = scale;
        transform.localScale = ls;

        Renderer.material.mainTextureOffset = new Vector2(0, timer * 20f);
    }
}
