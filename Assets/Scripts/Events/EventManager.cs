using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The event manager that holds customizable event dictionaries 
/// and manages the communication of invokers and listeners.
/// </summary>
public static class EventManager
{
    #region Fields

    // No argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> noArgEventInvokers =
    new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction>> noArgEventListeners =
        new Dictionary<EventType, List<UnityAction>>();

    // Single float argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> singleFloatEventInvokers = 
        new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction<float>>> singleFloatEventListeners = 
        new Dictionary<EventType, List<UnityAction<float>>>();

    // Single int argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> singleIntEventInvokers = 
        new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction<int>>> singleIntEventListeners = 
        new Dictionary<EventType, List<UnityAction<int>>>();

    // Single int argument event invokers and listeners
    static Dictionary<EventType, List<Invoker>> gameObjectEventInvokers = 
        new Dictionary<EventType, List<Invoker>>();
    static Dictionary<EventType, List<UnityAction<GameObject>>> gameObjectEventListeners = 
        new Dictionary<EventType, List<UnityAction<GameObject>>>();

    #endregion

    public static void Initialize()
    {
        // create empty lists for all the dictionary entries
        foreach (EventType name in Enum.GetValues(typeof(EventType)))
        {
            if (!singleFloatEventInvokers.ContainsKey(name))
            {
                singleFloatEventInvokers.Add(name, new List<Invoker>());
                singleFloatEventListeners.Add(name, new List<UnityAction<float>>());
            }
            else
            {
                singleFloatEventInvokers[name].Clear();
                singleFloatEventListeners[name].Clear();
            }

            if (!singleIntEventInvokers.ContainsKey(name))
            {
                singleIntEventInvokers.Add(name, new List<Invoker>());
                singleIntEventListeners.Add(name, new List<UnityAction<int>>());
            }
            else
            {
                singleIntEventInvokers[name].Clear();
                singleIntEventListeners[name].Clear();
            }

            if (!noArgEventInvokers.ContainsKey(name))
            {
                noArgEventInvokers.Add(name, new List<Invoker>());
                noArgEventListeners.Add(name, new List<UnityAction>());
            }
            else
            {
                noArgEventInvokers[name].Clear();
                noArgEventListeners[name].Clear();
            }

            if (!gameObjectEventInvokers.ContainsKey(name))
            {
                gameObjectEventInvokers.Add(name, new List<Invoker>());
                gameObjectEventListeners.Add(name, new List<UnityAction<GameObject>>());
            }
            else
            {
                gameObjectEventInvokers[name].Clear();
                gameObjectEventListeners[name].Clear();
            }
        }
    }

    #region No Argument Events

    /// <summary>
    /// Adds the invoker for the given no argument event type
    /// </summary>
    /// <param name="invoker">invoker</param>
    public static void AddNoArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeNoArgEventDict(eventType);
        
        foreach (UnityAction listener in noArgEventListeners[eventType])
        {
            invoker.AddNoArgumentListener(listener, eventType);
        }
        noArgEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given no argument event
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddNoArgumentListener(UnityAction listener, EventType eventType)
    {
        InitializeNoArgEventDict(eventType);

        foreach (Invoker invoker in noArgEventInvokers[eventType])
        {
            invoker.AddNoArgumentListener(listener, eventType);
        }
        noArgEventListeners[eventType].Add(listener);
    }

    /// <summary>
    /// Adds the event if it is not already in the respective dictionary.
    /// </summary>
    /// <param name="eventType"></param>
    private static void InitializeNoArgEventDict(EventType eventType)
    {
        if (!noArgEventListeners.ContainsKey(eventType))
        {
            noArgEventListeners.Add(eventType, new List<UnityAction>());
        }

        if (!noArgEventInvokers.ContainsKey(eventType))
        {
            noArgEventInvokers.Add(eventType, new List<Invoker>());
        }
    }
#endregion

    #region Float Argument Events

    /// <summary>
    /// Adds the invoker for the given float argument event type
    /// </summary>
    /// <param name="invoker">invoker</param>
    public static void AddFloatArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeFloatEventDict(eventType);

        foreach (UnityAction<float> listener in singleFloatEventListeners[eventType])
        {
            invoker.AddSingleFloatArgListener(listener, eventType);
        }
        singleFloatEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given float argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddFloatArgumentListener(UnityAction<float> listener, EventType eventType)
    {
        InitializeFloatEventDict(eventType);

        foreach (Invoker invoker in singleFloatEventInvokers[eventType])
        {
            invoker.AddSingleFloatArgListener(listener, eventType);
        }
        singleFloatEventListeners[eventType].Add(listener);
    }


    private static void InitializeFloatEventDict(EventType eventType)
    {
        if (!singleFloatEventListeners.ContainsKey(eventType))
        {
            singleFloatEventListeners.Add(eventType, new List<UnityAction<float>>());
        }

        if (!singleFloatEventInvokers.ContainsKey(eventType))
        {
            singleFloatEventInvokers.Add(eventType, new List<Invoker>());
        }
    }

#endregion

    #region GameObject Argument Listeners

    /// <summary>
    /// Adds the invoker for the given gameobject argument event type
    /// </summary>
    /// <param name="invoker">invoker</param>
    public static void AddGameObjectArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeGameObjectEventDict(eventType);

        foreach (UnityAction<GameObject> listener in gameObjectEventListeners[eventType])
        {
            invoker.AddGameObjectEventListener(listener, eventType);
        }
        
        gameObjectEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given gameobject argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddGameObjectArgumentListener(UnityAction<GameObject> listener, EventType eventType)
    {
        InitializeGameObjectEventDict(eventType);

        foreach (Invoker invoker in gameObjectEventInvokers[eventType])
        {
            invoker.AddGameObjectEventListener(listener, eventType);
        }
        gameObjectEventListeners[eventType].Add(listener);
    }

    private static void InitializeGameObjectEventDict(EventType eventType)
    {
        if (!gameObjectEventListeners.ContainsKey(eventType))
        {
            gameObjectEventListeners.Add(eventType, new List<UnityAction<GameObject>>());
        }

        if (!gameObjectEventInvokers.ContainsKey(eventType))
        {
            gameObjectEventInvokers.Add(eventType, new List<Invoker>());
        }
    }

    #endregion


    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddIntArgumentInvoker(Invoker invoker, EventType eventType)
    {
        InitializeIntEventDict(eventType);

        foreach (UnityAction<int> listener in singleIntEventListeners[eventType])
        {
            invoker.AddSingleIntEventListener(listener, eventType);
        }
        singleIntEventInvokers[eventType].Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddIntArgumentListener(UnityAction<int> listener, EventType eventType)
    {
        InitializeIntEventDict(eventType);

        foreach (Invoker invoker in singleIntEventInvokers[eventType])
        {
            invoker.AddSingleIntEventListener(listener, eventType);
        }
        singleIntEventListeners[eventType].Add(listener);
    }

    private static void InitializeIntEventDict(EventType eventType)
    {
        if (!singleIntEventListeners.ContainsKey(eventType))
        {
            singleIntEventListeners.Add(eventType, new List<UnityAction<int>>());
        }

        if (!singleIntEventInvokers.ContainsKey(eventType))
        {
            singleIntEventInvokers.Add(eventType, new List<Invoker>());
        }
    }
}
