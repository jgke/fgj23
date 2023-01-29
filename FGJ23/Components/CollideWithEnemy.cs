using FGJ23.Entities;
using FGJ23.Entities.Enemies;
using FGJ23.Support;
using FGJ23.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Components
{
    internal class CollideWithEnemy : Component, IUpdatable, ILoggable
    {
        public delegate bool Callback(Enemy enemy, bool hadContactOnPreviousFrame);
        public Callback Cb;
        public Dictionary<uint, bool> HadContactOnPreviousFrame = new();

        public CollideWithEnemy(Callback cb)
        {
            Cb = cb;
        }

        public void DrawUpdate() { }

        public virtual void FixedUpdate()
        {
            foreach (var kv in HadContactOnPreviousFrame)
            {
                HadContactOnPreviousFrame[kv.Key] = false;
            }

            var neighborColliders = ColliderSystem.CollideWithEnemies(Entity.GetComponent<Collider>());

            // loop through and check each Collider for an overlap
            foreach (var collider in neighborColliders)
            {
                var enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    var prev = HadContactOnPreviousFrame.GetValueOrDefault(enemy.Entity.Id, false);
                    HadContactOnPreviousFrame[enemy.Entity.Id] = true;
                    if (Cb(enemy, prev))
                    {
                        return;
                    }
                }
            }
        }
    }
}
