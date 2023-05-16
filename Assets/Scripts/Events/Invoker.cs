using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event invoker
/// </summary>
public class Invoker : MonoBehaviour
{
    protected Dictionary<EventType, NoArgumentEvent> noArgEventDict = 
        new Dictionary<EventType, NoArgumentEvent>();
    protected Dictionary<EventType, SingleFloatArgumentEvent> singleFloatArgEventDict = 
        new Dictionary<EventType, SingleFloatArgumentEvent>();
    protected Dictionary<EventType, SingleIntArgumentEvent> singleIntArgEventDict = 
        new Dictionary<EventType, SingleIntArgumentEvent>();
    protected Dictionary<EventType, GameObjectArgumentEvent> gameObjectArgEventDict = 
        new Dictionary<EventType, GameObjectArgumentEvent>();

    /// <summary>
    /// Adds the given listener to the no argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddNoArgumentListener(UnityAction listener, EventType eventType)
    {
        noArgEventDict[eventType].AddListener(listener);
    }
    /// <summary>
    /// Adds the given listener to the one argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddSingleFloatArgListener(UnityAction<float> listener, EventType eventType)
    {
        singleFloatArgEventDict[eventType].AddListener(listener);
    }

    public void AddSingleIntEventListener(UnityAction<int> listener, EventType eventType)
    {
        singleIntArgEventDict[eventType].AddListener(listener);
    }
    
    public void AddGameObjectEventListener(UnityAction<GameObject> listener, EventType eventType)
    {
        gameObjectArgEventDict[eventType].AddListener(listener);
    }

    /// <summary>
    /// Removes the given listener to the no argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void RemoveNoArgumentListener(UnityAction listener, EventType eventType)
    {
        noArgEventDict[eventType].RemoveListener(listener);
    }

    /// <summary>
    /// Removes the given listener to the one argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public void RemoveOneArgumentListener(UnityAction<float> listener, EventType eventType)
    {
        singleFloatArgEventDict[eventType].RemoveListener(listener);
    }

    public void InvokeNoArgumentEvent(EventType eventType)
    {
        noArgEventDict[eventType].Invoke();
    }

    public void InvokeSingleFloatArgEvent(EventType eventType, float argument)
    {
        singleFloatArgEventDict[eventType].Invoke(argument);
    }

    public void InvokeSingleIntArgEvent(EventType eventType, int argument)
    {
        singleIntArgEventDict[eventType].Invoke(argument);
    }

    public void InvokeGameObjectArgEvent(EventType eventType, GameObject argument)
    {
        gameObjectArgEventDict[eventType].Invoke(argument);
    }
}
