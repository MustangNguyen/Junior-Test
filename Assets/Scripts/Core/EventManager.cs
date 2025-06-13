using System;
using System.Collections.Generic;
using UnityEngine;

/* Brian
In the first game scene, create an empty Unity GameObject named EventManager and attach the EventManager.cs script to it. 
This script is set to DontDestroyOnLoad, i.e., it won't be destroyed when reloading scene.

=== HOW TO USE === 
- No parameter:
EventManager.Instance.TriggerEvent("gameOver", null);
- 1 parameter:
EventManager.Instance.TriggerEvent("gamePause", new Dictionary<string, object> { { "pause", true } });
- 2 or more parameters:
EventManager.Instance.TriggerEvent("addReward", 
  new Dictionary<string, object> {
    { "name", "candy" },
    { "amount", 5 } 
  });

=== How an event is published === 
void OnTriggerEnter2D(Collider2D other) {
    EventManager.Instance.TriggerEvent("addGolds", new Dictionary<string, object> { { "amount", 1 } });
  }

=== How an event is consumed example === 
public class Consumer : MonoBehaviour {
  private int Golds;

  void OnEnable() {
    EventManager.Instance.StartListening("addGolds", OnAddGolds);
  }

  void OnDisable() {
    EventManager.Instance.StopListening("addGolds", OnAddGolds);
  }
  
  void OnAddGolds(Dictionary<string, object> msg) {
    var amount = (int) msg["amount"];
    Golds += amount;
  }
}
*/
public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventName, Action<Dictionary<string, object>>> eventDictionary = new();

    public void StartListening(EventName eventName, Action<Dictionary<string, object>> listener)
    {
        // Debug.Log("Start Listening: " + eventName);
        Action<Dictionary<string, object>> thisEvent;

        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(EventName eventName, Action<Dictionary<string, object>> listener)
    {
        // Debug.Log("Stop Listening: " + eventName);
        Action<Dictionary<string, object>> thisEvent;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventName] = thisEvent;
        }
    }

    public void TriggerEvent(EventName eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // Debug.Log("Trigger Event Exist key: " + eventName);
            thisEvent?.Invoke(message);
        }
        else
        {
            // Debug.Log("Trigger Event Exist NOT  key: " + eventName);
        }
    }
}

public enum EventName
{
    GamePaused,
    GameResumed,
    TogglePause,
    GameOver,
    ScoreChanged,
    EnemyDied,
    DamageDealt
}