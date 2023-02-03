using FGJ23.Components;
using FGJ23.Support;
using FGJ23.Systems;
using Google.Protobuf;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Entities.AreaEvents
{
    internal class ForceMovement : AreaEvent, ILoggable
    {
        [Loggable]
        private Vector2 direction;
        private float speed;
        [Loggable]
        public Dictionary<uint, Vector2> Waypoint = new();

        public Vector2 Direction
        {
            get => direction; set
            {
                direction = value;
                speed = value.Length();
            }
        }

        public float Speed
        {
            get => speed;
        }

        public ForceMovement(ByteString data) : base("suck tube")
        {
            var reader = new BinaryBitReader(data.ToByteArray());
            var x = reader.ReadI8();
            var y = reader.ReadI8();

            Direction = 10 * new Vector2(x, y);
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            Entity.AddComponent(new CollideWithPlayer(this.HandleCollision));
        }

        // on first collision, set waypoint to middle
        // if waypoint; forcemove player towards waypoint with speed=|direction| 
        // else; forcemove player to direction
        // when pos - waypoint < deltatime*speed*2; waypoint = null
        // 
        private bool HandleCollision(Player player, bool hadContactOnPreviousFrame)
        {
            ForceMovementSystem.ForceMove(player, this);
            return false;
        }
    }
}
