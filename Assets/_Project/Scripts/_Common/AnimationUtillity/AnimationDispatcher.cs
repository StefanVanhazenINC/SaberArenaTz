using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts._Common.AnimationUtillity
{
    public class AnimationDispatcher : MonoBehaviour
    {
        private readonly Dictionary<string, List<Action>> _actionsDictionary = new();

        public void SubscribeOnEvent(string key, Action action)
        {
            if (!_actionsDictionary.ContainsKey(key))
            {
                _actionsDictionary[key] = new List<Action>();
            }

            _actionsDictionary[key].Add(action);
        }

        public void UnsubscribeOnEvent(string key, Action action)
        {
            if (_actionsDictionary.TryGetValue(key, out var actionsList))
            {
                actionsList.Remove(action);
            }
        }

        public void ReceiveEvent(string actionKey)
        {

            if (_actionsDictionary.TryGetValue(actionKey, out var actionsList))
            {
                foreach (var action in actionsList)
                {
                    action?.Invoke();
                }
            }
        }  
    }
}