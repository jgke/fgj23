using Google.Protobuf;
using FGJ23.Components;
using Nez.Sprites;
using Nez.Textures;
using Microsoft.Xna.Framework;
using FGJ23.Support;


namespace FGJ23.Entities.CoordinateEvents
{
    internal class FloatingBlob : CoordinateEvent
    {
        int width = 16;
        int height = 16;

        string spritepath;

        public FloatingBlob(ByteString data) : base("blob") {
            spritepath = data.ToStringUtf8();
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/Files/" + spritepath);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

            if(sprites.Count <= 4) {
                SpriteAnimator animator = Entity.AddComponent(new SpriteAnimator(sprites[1]));;
                animator.AddAnimation("Idle", new[] { sprites[1] });
                animator.Play("Idle");
            } else {
                SpriteAnimator animator = Entity.AddComponent(new SpriteAnimator(sprites[7]));;
                animator.AddAnimation("Idle", new[] { sprites[7] });
                animator.Play("Idle");
            }
            Entity.AddComponent(new Enemy());
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
                    FmodWrapper.PlaySound("event:/Vihollinenlitistyy");
                player._rigidBody.velocity = new Vector2(player._rigidBody.velocity.X, -500);
            }
            else
            {
                Log.Information("Missed");
                GameplayScene.ResetLevel();
            }
            Entity.Destroy();

            return false;
        }
    }
}
