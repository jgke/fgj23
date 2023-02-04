using FGJ23.Components;
using FGJ23.Support;
using FGJ23.Systems;
using FGJ23.Levels;
using Google.Protobuf;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Textures;

namespace FGJ23.Entities.AreaEvents
{
    internal class LevelEnd : AreaEvent, ILoggable
    {
        private string image;
        private string next;

        public LevelEnd(ByteString data) : base("goto next")
        {
            string[] d = data.ToStringUtf8().Split(";");

            image = d[0];
            next = d[1];
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            Entity.AddComponent(new CollideWithPlayer(this.HandleCollision));

            var texture = Entity.Scene.Content.LoadTexture("Content/Files/" + image);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);

            SpriteAnimator animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));;
            animator.AddAnimation("Idle", new[] { sprites[0] });
            animator.Play("Idle");
        }

        private bool HandleCollision(Player player, bool hadContactOnPreviousFrame)
        {
            if(player.PreventActions) {
                return true;
            }
            player.PreventActions = true;
            var storyEntity = player.Entity.Scene.CreateEntity("endStory", new Vector2(0, 0));
            storyEntity.AddComponent(LevelBank.getEndStory(GameplayScene.NextProtoLevel.Name));
            GameState.OnStoryComplete += () =>
            {
                GameState.Instance.DoTransition(() =>
                {
                    var lev = LevelBank.GetLevel(next);
                    if (lev != null)
                    {
                        Log.Information("new GS {A}", lev);
                        return GameplayScene.construct(lev);
                    }
                    else
                    {
                        Log.Information("goto menu");
                        return new MenuScene();
                    }
                });
            };
            Entity.Destroy();
            return true;
        }
    }
}
