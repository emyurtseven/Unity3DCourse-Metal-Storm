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
    static List<Health> enemyDeadInvokers = new List<Health>();
    static UnityAction<int> pointAddedListener;
    static UnityAction<GameObject> enemyDeadListener;


    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddPointAddedInvoker(Health invoker)
    {
        if (pointAddedListener != null)
        {
            invoker.AddPointsAddedEventListener(pointAddedListener);
        }

        pointAddedInvokers.Add(invoker);
    }

    public static void AddEnemyDeadInvoker(Health invoker)
    {
        if (enemyDeadListener != null)
        {
            invoker.AddEnemyDeadEventListener(enemyDeadListener);
        }

        enemyDeadInvokers.Add(invoker);
    }

    /// <summary>
    /// Adds the listener for the given int argument event type
    /// </summary>
    /// <param name="listener">listener</param>
    public static void AddPointAddedListener(UnityAction<int> handler)
    {
        pointAddedListener = handler;

        foreach (Health invoker in pointAddedInvokers)
        {
            invoker.AddPointsAddedEventListener(pointAddedListener);
        }
    }
    public static void AddEnemyDeadListener(UnityAction<GameObject> handler)
    {
        enemyDeadListener = handler;

        foreach (Health invoker in enemyDeadInvokers)
        {
            invoker.AddEnemyDeadEventListener(enemyDeadListener);
        }
    }


}
