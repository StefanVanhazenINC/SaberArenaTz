using UnityEngine;

namespace _Project.Scripts.Skills
{
    public struct SkillTarget
    {
        public Collider Collider;
        public Transform Root;
        public Vector3 Point;

        public SkillTarget(Collider collider)
        {
            Collider = collider;
            Root = collider.transform.root;
            Point = collider.bounds.center;
        }
    }
}
