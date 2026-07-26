using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    [Header("Enter Level Zoom")]
    [SerializeField] private Ease.Type easing;
    [SerializeField] private float zoomStart = 30f;
    [SerializeField] private float zoomEnd = 10.8f;

    private float enterLevelZoomDuration = 2f;
    private bool processEnterLevelZoom = false;
    private float processEnterLevelProgress = 0f;

    // Update is called once per frame
    void Update()
    {
        if (processEnterLevelZoom)
        {
            processEnterLevelProgress += Time.deltaTime;
            if (processEnterLevelProgress >= enterLevelZoomDuration)
            {
                StopEnterLevelZoom();
            } else
            {
                // Progress Zoom
                float zoomProgress = processEnterLevelProgress / enterLevelZoomDuration;
                zoomProgress = Ease.OfType(zoomProgress, easing);
                GetComponent<Camera>().orthographicSize = zoomStart - (zoomStart - zoomEnd) * zoomProgress;
            }
        }
    }

    public void EnterLevelZoom(float duration = 0f)
    {
        if (duration > 0f)
        {
            GetComponent<Camera>().orthographicSize = zoomStart;
            enterLevelZoomDuration = duration;
            processEnterLevelProgress = 0f;
            processEnterLevelZoom = true;
        } else
        {
            StopEnterLevelZoom();
        }
        
    }

    public void StopEnterLevelZoom()
    {
        GetComponent<Camera>().orthographicSize = zoomEnd;
        processEnterLevelZoom = false;
    }
}
