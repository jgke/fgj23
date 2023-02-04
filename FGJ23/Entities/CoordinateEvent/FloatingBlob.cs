using Google.Protobuf;
using FGJ23.Components;
using Nez.Sprites;
using Microsoft.Xna.Framework;

namespace FGJ23.Entities.CoordinateEvents
{
    internal class FloatingBlob : CoordinateEvent
    {
        int width = 32;
        int height = 32;

        public FloatingBlob(ByteString data) : base("blob") { }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(Content.Files.FloatingBlob);
            Entity.AddComponent(new Enemy());
            Entity.AddComponent(new SpriteRenderer(texture));
            Entity.AddComponent(new BoxCollider(-width / 2, -height / 2, width, height));
            Entity.AddComponent(new CollideWithPlayer(this.OnCollide));
            Entity.AddComponent(new Health(2));
        }

        private bool OnCollide(Player player, bool hadContactOnPreviousFrame)
        {
            Log.Information("Blob collided with player");
            var prevP = player.Entity.PreviousTransform.Position;
            var curP = player.Entity.Position;
            var prevE = this.Entity.PreviousTransform.Position;
            var curE = this.Entity.Position;

            var prevPlayerBL = new Vector2(prevP.X, prevP.Y + player.Height);
            var prevPlayerBR = new Vector2(prevP.X + player.Width, prevP.Y + player.Height);
            var curPlayerBL = new Vector2(curP.X, curP.Y + player.Height);
            var curPlayerBR = new Vector2(curP.X + player.Width, curP.Y + player.Height);
            var bld = curPlayerBL - prevPlayerBL;
            var brd = curPlayerBR - prevPlayerBR;

            var curETL = new Vector2(curE.X, curE.Y);
            var curETR = new Vector2(curE.X + width, curE.Y);

            Log.Information("Colliding line ({A}->{B}) with ({C}->{D}))", prevPlayerBL, curPlayerBL + bld, curETL, curETR);
            Log.Information("Colliding line ({A}->{B}) with ({C}->{D}))", prevPlayerBR, curPlayerBR + brd, curETL, curETR);

            if (Collisions.LineToLine(prevPlayerBL, curPlayerBL + bld, curETL, curETR)
                    || Collisions.LineToLine(prevPlayerBR, curPlayerBR + bld, curETL, curETR))
            {
                Log.Information("Collided! Jumping...");
                player._rigidBody.velocity = new Vector2(player._rigidBody.velocity.X, -500);
            }
            else
            {
                Log.Information("Missed");
                player.GetComponent<Health>().Hit(1);
            }
            Entity.Destroy();

            return false;
        }
    }
}
