#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;

public static class EditorStorage
{
    public static readonly Dictionary<string, MethodInfo> OnChangeMethods = new Dictionary<string, MethodInfo>();
    public static readonly Dictionary<string, List<MethodInfo>> RhuInspectorMethods = new Dictionary<string, List<MethodInfo>>();
    public static readonly Dictionary<string, FieldInfo> RhuInspectorFields = new Dictionary<string, FieldInfo>();
    
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void Refresh()
    {
        OnChangeMethods.Clear();
        RhuInspectorMethods.Clear();
        RhuInspectorFields.Clear();
    }
}
#endif
