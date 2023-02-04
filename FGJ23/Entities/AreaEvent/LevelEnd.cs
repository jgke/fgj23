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

namespace FGJ23.Entities.AreaEvents
{
    internal class LevelEnd : AreaEvent, ILoggable
    {
        private string next;

        public LevelEnd(ByteString data) : base("goto next")
        {
            this.next = data.ToStringUtf8();
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            Entity.AddComponent(new CollideWithPlayer(this.HandleCollision));
            Entity.AddComponent(new DrawRectangle((int)size.X, (int)size.Y, new Color(30, 30, 150, 150)));
        }

        private bool HandleCollision(Player player, bool hadContactOnPreviousFrame)
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
            Entity.Destroy();
            return true;
        }
    }
}
