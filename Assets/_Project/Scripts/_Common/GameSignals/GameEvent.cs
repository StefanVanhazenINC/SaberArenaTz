using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts._Common.GameSignals
{
    [CreateAssetMenu(menuName ="Game Event",fileName = "New Game Event")]
    public class GameEvent : ScriptableObject
    {
        HashSet<GameEventListener> _listeners = new HashSet<GameEventListener>();

        public void Invoke() 
        {
            foreach (GameEventListener globalEventListener in _listeners)
            {
                globalEventListener.RaiseEvent();
            }
        }

        public void Register(GameEventListener gameEventListener) 
        {
            _listeners.Add(gameEventListener);
        }
        public void Deregister(GameEventListener gameEventListener) 
        {
            _listeners.Remove(gameEventListener);
        }
    }

}