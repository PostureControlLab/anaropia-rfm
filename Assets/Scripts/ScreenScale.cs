using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScale : MonoBehaviour, IScaler
{
    public Transform screen;

    [Range(0.001f, 10)]
    public float currentScale = 1;
    public bool doScaleTexture = true;

    public Renderer screenRenderer;

    private Vector3 screenStartPos;
    private Vector3 screenStartScale;
    private Vector2 initialTextureScale;
    private float currentDistance;

    private Vector3 scalePosition;


    private void Start()
    {
        screenStartPos = screen.position;
        screenStartScale = screen.localScale;
        
        initialTextureScale = screenRenderer.material.mainTextureScale;

        CalibrateEyeHeight();
        SetScreenScale(currentScale);
    }

    private void Update()
    {
        //SetScreenScale(currentScale);
    }

    public void SetScreenScale(float scale)
    {
        if (scale == 0.0f) scale = 0.00000001f;

        currentScale = scale;

        ScaleAroundWorld(screen, scalePosition, Vector3.one * scale);

        if (doScaleTexture)
        {
            screenRenderer.material.mainTextureScale = initialTextureScale * currentScale;
        }
    }

    public void SetScale(float scale)
    {
        SetScreenScale(scale);
    }

    public void CalibrateEyeHeight()
    {
        scalePosition = new Vector3(screen.position.x, Camera.main.transform.position.y, screen.position.z);
    }

    private void ScaleAround(Transform target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.localScale = newScale;
        target.localPosition = FP;
    }

    private void ScaleAroundWorld(Transform target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.position;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.localScale = newScale;
        target.position = FP;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(650, 10, 150, 35), "Calibrate eye height"))
        {
            CalibrateEyeHeight();
        }
    }

}
