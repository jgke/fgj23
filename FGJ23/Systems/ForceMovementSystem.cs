using FGJ23.Entities;
using FGJ23.Entities.AreaEvents;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Systems
{
    internal class ForceMovementSystem : Component
    {
        private static Dictionary<uint, bool> BeingForceMoved = new();
        private static Dictionary<uint, Vector2> CurrentSpeed = new();

        public static void ForceMove(Player player, ForceMovement force)
        {
            var pId = player.Entity.Id;
            Vector2 direction = force.Direction;

            if (!BeingForceMoved.GetValueOrDefault(pId, false) || !CurrentSpeed.ContainsKey(pId))
            {
                Log.Information("Initial contact");
                BeingForceMoved[pId] = true;
                CurrentSpeed[pId] = force.Direction;
                ColliderSystem.SetPosition(player.GetComponent<Collider>(), force.Transform.Position + new Vector2(16, 16));
            }

            if ((CurrentSpeed[pId].X != 0 && force.Direction.X == 0) || (CurrentSpeed[pId].Y != 0 && force.Direction.Y == 0))
            {
                CurrentSpeed[pId] = force.Direction;
                ColliderSystem.SetPosition(player.GetComponent<Collider>(), force.Transform.Position + new Vector2(16, 16));
            }

            RigidBody body = player.GetComponent<RigidBody>();
            player.LockControls(0.3f);
            body.ForceMove(direction, 0.3f, 0.3f);
        }

        public static void StopForceMove(Player player)
        {
            BeingForceMoved.Remove(player.Entity.Id);
            CurrentSpeed.Remove(player.Entity.Id);
        }
    }
}
