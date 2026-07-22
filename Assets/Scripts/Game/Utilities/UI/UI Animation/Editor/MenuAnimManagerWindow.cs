using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(StandaloneUIAnimSequence))]
public class MenuAnimManagerWindow : RhuInspector
{
    public override void OnInspectorGUI()
    {
        StandaloneUIAnimSequence animSequencePlayer = (StandaloneUIAnimSequence)target;
        
        DrawRhuInspector();
        
        GUILayout.Space(10);
        
        if(GUILayout.Button("Preview Appear", GUILayout.MaxHeight(30)))
        {
            animSequencePlayer.Appear(); 
        }
        if(GUILayout.Button("Preview Disappear", GUILayout.MaxHeight(30)))
        {
            animSequencePlayer.Disappear(); 
        }if(GUILayout.Button("Skip", GUILayout.MaxHeight(30)))
        {
            animSequencePlayer.SkipSequence(); 
        }
    }
}
