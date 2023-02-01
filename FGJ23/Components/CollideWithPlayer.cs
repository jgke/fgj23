using FGJ23.Entities;
using FGJ23.Support;
using FGJ23.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Components
{
    internal class CollideWithPlayer : Component, IUpdatable, ILoggable
    {
        public delegate bool Callback(Player player, bool hadContactOnPreviousFrame);
        public Callback Cb;
        public Dictionary<uint, bool> HadContactOnPreviousFrame = new();

        public CollideWithPlayer(Callback cb)
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

            var neighborColliders = ColliderSystem.CollideWithPlayers(Entity.GetComponent<Collider>());

            // loop through and check each Collider for an overlap
            foreach (var collider in neighborColliders)
            {
                var player = collider.GetComponent<Player>();
                if (player != null)
                {
                    var prev = HadContactOnPreviousFrame.GetValueOrDefault(player.Entity.Id, false);
                    HadContactOnPreviousFrame[player.Entity.Id] = true;
                    if (Cb(player, prev))
                    {
                        return;
                    }
                }
            }
        }
    }
}
