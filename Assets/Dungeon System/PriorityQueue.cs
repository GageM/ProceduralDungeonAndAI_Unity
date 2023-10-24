using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// A Priority Queue Class using List<T> as a base since C# doesn't have a built in Priority Queue
[Serializable]
public class PriorityQueue<T>
{
    List<prQueueValue<T>> queue;

    // Gets the List containing the queue values
    public List<prQueueValue<T>> get { get { return queue; } }

    public PriorityQueue()
    {
        queue = new List<prQueueValue<T>>();
    }

    public void Enqueue(T value, float weight)
    {
        int index = -1;

        // Loop from highest weight to lowest & check if new weight is less than the weight at index i
        for (int i = queue.Count - 1; i >= 0; i--)
        {
            if (weight < queue[i].weight)
            {
                index = i;
            }
        }

        if (index == -1)
        {
            queue.Add(new(value, weight));
        }
        else
        {
            queue.Insert(index, new(value, weight));
        }
    }

    public T Dequeue()
    {
        // Get the last element of the queue and then remove it from the queue
        var val = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);

        return val.value;
    }

    public T Peek()
    {
        var val = queue[queue.Count - 1];
        return val.value;
    }
}

// A struct to hold the weight and value of an object in a priority queue
[Serializable]
public struct prQueueValue<t>
{
    public prQueueValue(t v, float w) { value = v; weight = w; }
    public t value;
    public float weight;
}






