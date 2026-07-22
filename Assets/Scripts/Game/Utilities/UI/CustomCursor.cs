using System.Collections;
using UnityEngine;

public class CustomCursor : UIAnimations
{
    public static CustomCursor instance;

    private int ongoingAnim;
    
    private bool visible;
    public bool Visible
    {
        get => visible;
        set
        {
            if (value == visible) return;
            if (!cursor) return;

            SkipAnim(ongoingAnim);
            ongoingAnim = PlayAnim(value ? appearAnim : disappearAnim, value)[0];
            
            visible = value;
            Cursor.visible = false;
        }
    }
    
    [SerializeField] private RectTransform cursor;
    private Camera cam;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float lerpSpeed = 5;
    private Vector3 mousePosition;
    [Header("Animation")] 
    public Anim appearAnim;
    public Anim disappearAnim;

    private void Awake()
    {
        instance = this;
        cam = canvas.worldCamera;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 scale = new Vector2(cam.pixelRect.width / canvas.scaleFactor, cam.pixelRect.height / canvas.scaleFactor);
        mousePosition = Input.mousePosition;
        mousePosition = new Vector3((Mathf.Clamp01(cam.ScreenToViewportPoint(mousePosition).x) - 0.5f) * scale.x, (Mathf.Clamp01(cam.ScreenToViewportPoint(mousePosition).y) - 0.5f) * scale.y, 0);
        cursor.localPosition = Vector3.Lerp(cursor.localPosition, new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, 0), Time.unscaledDeltaTime * lerpSpeed);
    }

    public void SetCamera(Camera c)
    {
        canvas.worldCamera = cam = c;
    }

    public void Lock()
    {
        if (!cursor) return;
        StartCoroutine(LockAfterDelay());
    }
    private IEnumerator LockAfterDelay()
    {
        yield return new WaitForSecondsRealtime(disappearAnim.duration);
        Cursor.lockState = CursorLockMode.Locked;
    }
}

public static class C_Cursor
{
    private static CustomCursor cu;

    private static CustomCursor Cu()
    {
        if (!cu)
            cu = CustomCursor.instance;
        return cu;
    }

    public static bool Visible()
    {
        return Cu().Visible;
    }
    
    public static void HideAndLock()
    {
        if (!Cu()) return;
        
        if (!Cu().Visible) return;
        
        Cu().Visible = false;
        Cu().Lock();
    }
    
    public static void ShowAndFree()
    {
        if (!Cu()) return;
        if (ControllerDetection.instance?.ControlType != ControlType.Keyboard) return;
        
        Cu().Visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void SetCamera(Camera c)
    {
        Cu().SetCamera(c);
    }
}