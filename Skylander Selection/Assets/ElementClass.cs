using UnityEngine;
using static Enums;

public class ElementClass
{
    public Sprite Sprite;
    public ElementEnum Element;

    public ElementClass(Sprite sprite, ElementEnum elementEnum)
    {
        Sprite = sprite;
        Element = elementEnum;
    }
}