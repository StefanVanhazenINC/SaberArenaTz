using Common.BaseComponent;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.Data
{
    public struct BulletInfo
    {
        public Vector3 direction;
        public float force;
        public int damage;
        public float speed;
        public float lifeTime;
        public TeamComponent team;

        public float size;
        public GameObject visual;
        public BulletInfo(Vector3 direction, int damage,float force, float speed, float lifeTime, TeamComponent team,GameObject visual = null, float size = 1)
        {
            this.direction = direction;
            this.damage = damage;
            this.speed = speed;
            this.lifeTime = lifeTime;
            this.team = team;
            this.size = size;
            this.visual = visual;
            this.force = force;
        }
    }
}