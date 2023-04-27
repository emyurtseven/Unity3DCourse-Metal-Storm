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
    static Health invoker;
    static UnityAction<int> listener;


    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddInvoker(Health script)
    {
        invoker = script;
        if (listener != null)
        {
            invoker.AddPointsAddedEventListener(listener);
        }
    }

    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddListener(UnityAction<int> handler)
    {
        listener = handler;

        if (invoker != null)
        {
            invoker.AddPointsAddedEventListener(listener);
        }
    }
}
