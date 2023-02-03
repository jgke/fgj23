using FGJ23.Entities;
using FGJ23.Entities.Enemies;
using Microsoft.Xna.Framework;
using Nez.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Systems
{
    public class ColliderSystem
    {
        public const int AllLayers = -1;

        public static int SpatialHashCellSize = 100;
        static SpatialHash PlayerPool;
        static SpatialHash EnemyPool;

        public static void Reset()
        {
            if (PlayerPool == null)
            {
                Log.Warning("Resetting ColliderSystem");
                PlayerPool = new SpatialHash(SpatialHashCellSize);
                EnemyPool = new SpatialHash(SpatialHashCellSize);
            }
        }

        public static void RegisterCollider(Entity entity)
        {
            var p = entity.GetComponent<Player>();
            if (p != null)
            {
                PlayerPool.Register(entity.GetComponent<Collider>());
            }

            var e = entity.GetComponent<Enemy>();
            if (e != null)
            {
                EnemyPool.Register(entity.GetComponent<Collider>());
            }
        }

        public static void UnRegisterCollider(Entity entity)
        {
            var p = entity.GetComponent<Player>();
            if (p != null)
            {
                PlayerPool.Remove(entity.GetComponent<Collider>());
            }

            var e = entity.GetComponent<Enemy>();
            if (e != null)
            {
                EnemyPool.Remove(entity.GetComponent<Collider>());
            }
        }

        public static HashSet<Collider> CollideWithPlayers(Collider collider, int layerMask = AllLayers)
        {
            var bounds = collider.Bounds;
            return PlayerPool.AabbBroadphase(ref bounds, collider, layerMask);
        }

        public static HashSet<Collider> CollideWithEnemies(Collider collider, int layerMask = AllLayers)
        {
            var bounds = collider.Bounds;
            return EnemyPool.AabbBroadphase(ref bounds, collider, layerMask);
        }

        public static void ForceMove(Collider collider, Vector2 motion)
        {
            UnRegisterCollider(collider.Entity);
            collider.UnregisterColliderWithPhysicsSystem();
            collider.Entity.Transform.Position += motion;
            collider.RegisterColliderWithPhysicsSystem();
            RegisterCollider(collider.Entity);
        }

        public static void SetPosition(Collider collider, Vector2 position)
        {
            UnRegisterCollider(collider.Entity);
            collider.UnregisterColliderWithPhysicsSystem();
            collider.Entity.Transform.Position = position + collider.LocalOffset;
            collider.RegisterColliderWithPhysicsSystem();
            RegisterCollider(collider.Entity);
        }
    }
}
