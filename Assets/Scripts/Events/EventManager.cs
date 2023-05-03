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
    static List<Health> pointAddedInvokers = new List<Health>();
    static UnityAction<int> listener;


    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddInvoker(Health invoker)
    {
        if (listener != null)
        {
            invoker.AddPointsAddedEventListener(listener);
        }

        pointAddedInvokers.Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddListener(UnityAction<int> handler)
    {
        listener = handler;

        foreach (Health invoker in pointAddedInvokers)
        {
            invoker.AddPointsAddedEventListener(listener);
        }
    }
}
