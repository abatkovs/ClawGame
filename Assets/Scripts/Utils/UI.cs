using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    public static VisualElement Create(params string[] className)
    {
        return Create<VisualElement>(className);
    }
    
    //Create UIElement and assign style classes to it
    public static T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var uiElement = new T();
        foreach (var className in classNames)
        {
            uiElement.AddToClassList(className);
        }
        return uiElement;
    }
}
