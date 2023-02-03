using FGJ23.Components;
using Nez.Sprites;

namespace FGJ23.Entities.Enemies
{
    internal class FloatingBlob : Component
    {
        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(Content.Files.FloatingBlob);
            Entity.AddComponent(new Enemy());
            Entity.AddComponent(new SpriteRenderer(texture));
            Entity.AddComponent(new BoxCollider(-16, -16, 32, 32));
            Entity.AddComponent(new CollideWithPlayer(this.OnCollide));
            Entity.AddComponent(new Health(2));
        }

        private bool OnCollide(Player player, bool hadContactOnPreviousFrame)
        {
            player.GetComponent<Health>().Hit(1);
            return false;
        }
    }
}
