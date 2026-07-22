using UnityEngine;

public class FoldHeaderAttribute : PropertyAttribute
{
    public readonly string Header;

    public FoldHeaderAttribute(string header)
    {
        Header = header;
    }
}
