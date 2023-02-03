using FGJ23.Components;
using FGJ23.Support;
using Google.Protobuf;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Entities.CoordinateEvents
{
    public class HealthPickup : CoordinateEvent, ILoggable
    {
        private SpriteAnimator animator;

        public HealthPickup() : base("car")
        {
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            var texture = Entity.Scene.Content.LoadTexture(Content.Files.HealthPickup);
            var sprites = Sprite.SpritesFromAtlas(texture, 8, 8);
            var width = 8;
            var height = 8;
            Entity.AddComponent(new BoxCollider(-width / 2, -height / 2, width, height));

            animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            animator.AddAnimation("Idle", sprites.GetRange(0, 12).ToArray());
            animator.Play("Idle");

            Entity.AddComponent(new CollideWithPlayer(this.HandleCollision));
        }

        private bool HandleCollision(Player player, bool hadContactOnPreviousFrame)
        {
            Entity.Destroy();
            return true;
        }
    }
}
