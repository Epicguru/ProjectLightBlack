using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditingCameraControls : MonoBehaviour
{
    public Camera Camera
    {
        get
        {
            if (_cam == null)
                _cam = GetComponent<Camera>();
            return _cam;
        }
    }
    private Camera _cam;

    public Vector2 CameraZoomBounds = new Vector2(3, 60);

    private float lerpSpeed = 20f;
    private float targetZoom;

    private void Awake()
    {
        targetZoom = Camera.orthographicSize;
    }

    private void Update()
    {
        float scrollDelta = -Input.mouseScrollDelta.y;

        if(scrollDelta != 0f)
            targetZoom *= scrollDelta > 0f ? 1.1f : 0.9f;
        targetZoom = Mathf.Clamp(targetZoom, CameraZoomBounds.x, CameraZoomBounds.y);

        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, targetZoom, Time.unscaledDeltaTime * lerpSpeed);
    }
}
