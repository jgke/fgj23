using FGJ23.Support;
using FGJ23.Systems;

namespace FGJ23.Components
{
    internal class Damage : Component, IUpdatable, ILoggable
    {
        [Loggable]
        public int OnHitDamage;
        public bool DestroyOnHit = true;

        public Entity Source;

        public Damage(int damage, Entity source)
        {
            OnHitDamage = damage;
            Source = source;
        }

        public void DrawUpdate() { }

        public virtual void FixedUpdate()
        {
            var neighborColliders = ColliderSystem.CollideWithPlayers(Source.GetComponent<Collider>());

            // loop through and check each Collider for an overlap
            foreach (var collider in neighborColliders)
            {
                if (!Entity.IsDestroyed && Entity.GetComponent<Collider>().Overlaps(collider))
                {
                    DoCollision(collider.Entity);
                }
            }
        }

        private void DoCollision(Entity other)
        {

            Health otherHealth = other.GetComponent<Health>();

            if (otherHealth != null)
            {
                otherHealth.Hit(OnHitDamage);
            }
            else
            {
                Log.Warning("Hit with a Player which didn't have Health");
                return;
            }

            if (DestroyOnHit)
            {
                Entity.Destroy();
            }
            else
            {
                // callback here or sth
            }
        }
    }
}
