using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneReference
{
    [SerializeField] private string name;
    
    
    public static implicit operator int(SceneReference sceneRef)
    {
        return SceneManager.GetSceneByName(sceneRef.name).buildIndex;
    }
    
    public static implicit operator string(SceneReference sceneRef)
    {
        return sceneRef.name;
    }

    public static implicit operator bool(SceneReference sceneRef)
    {
        return SceneManager.GetActiveScene().name == sceneRef.name;
    }

    
    public static implicit operator SceneReference(int sceneIndex)
    {
        var reference = new SceneReference
        {
            name = SceneManager.GetSceneByBuildIndex(sceneIndex).name
        };

        return reference;
    }
    
    public static implicit operator SceneReference(string sceneName)
    {
        var reference = new SceneReference
        {
            name = sceneName
        };

        return reference;
    }
}
