using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character
{
    public class PlayerMarker : MonoBehaviour
    {
        [SerializeField] private GameObjectContext _context;

        
        public void RunContext()
        {
            _context.Run();
        }
    }
}