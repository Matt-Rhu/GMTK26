using UnityEngine;
using UnityEngine.SceneManagement;

public static class Inputs
{
    private static InputMap map;
    public static InputMap Map
    {
        get
        {
            if (map == null)
            {
                map = new InputMap();
                map.Enable();
            }
            return map;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Subscribe()
    {
        SceneManager.sceneUnloaded += DestroyMap;
    }
    private static void DestroyMap(Scene s)
    {
        map.Dispose();
        map = null;
    }

    public static InputMap.GameplayActions Gameplay => Map.Gameplay;
}
