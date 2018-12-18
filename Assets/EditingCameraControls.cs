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
    private Vector2 lastMousePos;
    private bool inDrag = false;

    private void Awake()
    {
        targetZoom = Camera.orthographicSize;
    }

    private void Update()
    {
        // Zoom...
        float scrollDelta = -Input.mouseScrollDelta.y;

        if(scrollDelta != 0f)
            targetZoom *= scrollDelta > 0f ? 1.1f : 0.9f;
        targetZoom = Mathf.Clamp(targetZoom, CameraZoomBounds.x, CameraZoomBounds.y);

        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, targetZoom, Time.unscaledDeltaTime * lerpSpeed);

        // Pan...
        if(InputManager.IsPressed("Camera Pan"))
        {
            if(inDrag == false)
            {
                inDrag = true;
                lastMousePos = InputManager.ScreenMousePos;
            }          

            Vector2 current = InputManager.ScreenMousePos;
            Vector2 delta = current - lastMousePos;

            delta /= new Vector2(Screen.width, Screen.height);
            float ratio = Screen.width / (float)Screen.height;
            delta *= new Vector2(Camera.orthographicSize * ratio * 2f, Camera.orthographicSize * 2f);

            lastMousePos = current;

            Camera.transform.Translate(-delta);
        }
        else
        {
            inDrag = false;
        }
    }
}
