using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    [SerializeField]
    public State initialState;
    
    State currentState;

    // Cache components needed for FSM classes here
    private Dictionary<System.Type, Component> componentCache;

    private Dictionary<string, List<GameObject>> taggedObjectCache;

    // Start is called before the first frame update
    void Start()
    {
        // Do all entry actions for the initial state
        if (currentState.entryActions.Count > 0)
        {
            foreach (var action in currentState.entryActions)
            {
                action.Act(this);
            }
        }
    }


    void Awake()
    {
        currentState = initialState;
        componentCache = new();
        taggedObjectCache = new();
    }

    // FixedUpdate is called at set intervals regardless of frame rate
    void FixedUpdate()
    {
        if (currentState.transitions.Count > 0)
        {
            // Check if any transitions have been triggered
            foreach (var transition in currentState.transitions)
            {
                if (transition.Test(this))
                {
                    TransitionState(transition);
                    return;
                }
            }
        }

        if (currentState.actions.Count > 0)
        {
            // Perform actions for current state
            foreach (var action in currentState.actions)
            {
                action.Act(this);
            }
        }
    }

    void TransitionState(Transition transition)
    {
        // Do all exit actions before changing the state
        if (currentState.exitActions.Count > 0)
        {
            foreach(var action in currentState.exitActions)
            {
                action.Act(this);
            }
        }

        // Transition to the next state
        currentState = transition.targetState;

        // Do all entry actions for the new state
        if (currentState.entryActions.Count > 0)
        {
            foreach (var action in currentState.entryActions)
            {
                action.Act(this);
            }
        }
    }

    // A custom function that caches components returned by the base GetComponent function for use in FSM classes
    public new T GetComponent<T>() where T : Component
    {
        if(componentCache.ContainsKey(typeof(T)))
        {
            return componentCache[typeof(T)] as T;
        }


        var component = base.GetComponent<T>();

        if(component != null)
        {
            componentCache.Add(typeof(T), component);
        }
        return component;
    }

    public List<GameObject> GetGameObjectsByTag(string tag)
    {
        if(taggedObjectCache.ContainsKey(tag))
        {
            return taggedObjectCache[tag];
        }

        List<GameObject> gameObjects = new();        
        foreach(var gameObject in GameObject.FindGameObjectsWithTag(tag))
        {
            gameObjects.Add(gameObject);
        }

        taggedObjectCache.Add(tag, gameObjects);
        return gameObjects;
    }
}






