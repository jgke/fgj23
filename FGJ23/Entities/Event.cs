using FGJ23.Entities.AreaEvents;
using FGJ23.Entities.CoordinateEvents;
using FGJ23.Levels;
using Google.Protobuf;
using Microsoft.Xna.Framework;
using Nez.UI;
using System;
using System.Collections;
using FGJ23.Support;

namespace FGJ23.Entities
{
    public abstract class EventBase : Component
    {
        [Loggable]
        public readonly string name;
        [Loggable]
        public Vector2 position;

        public EventBase(String name)
        {
            this.name = name;
        }

        public override void Initialize()
        {
            base.Initialize();
            Entity.Transform.Position = position;
        }

        public abstract Rectangle GetRect(Vector2 topLeft, Level level);

        public virtual void Draw(Batcher batcher, Vector2 topLeft, Level level)
        {
            var tint = Color.FromNonPremultiplied(0x8a, 0x2b, 0xe2, 100);
            batcher.DrawRect(GetRect(topLeft, level), tint);
        }
        public virtual void DrawText(Batcher batcher, Vector2 topLeft, Level level)
        {
            var l = new Label(this.name);
            var evRect = GetRect(topLeft, level);
            var scale = level.Entity.GraphicsTransform.Scale;
            l.x = evRect.X / scale.X;
            l.y = evRect.Y / scale.Y;
            l.SetWidth(level.TileSize.X * scale.X);
            l.SetHeight(level.TileSize.Y * scale.X);
            l.SetWrap(true);
            l.SetAlignment(Align.Center, Align.Center);
            l.Layout();
            l.Draw(batcher, 1);
        }
    }
    public class CoordinateEvent : EventBase
    {
        public CoordinateEvent(string name) : base(name)
        {
        }

        public override void Draw(Batcher batcher, Vector2 topLeft, Level level)
        {
            base.Draw(batcher, topLeft, level);
            DrawText(batcher, topLeft, level);
        }

        public override Rectangle GetRect(Vector2 topLeft, Level level)
        {
            var tsHalf = new Point(level.TileSize.X / 2, level.TileSize.Y / 2);
            return new Rectangle((this.position + topLeft).RoundToPoint() - tsHalf, level.TileSize);

        }
    }

    public class AreaEvent : EventBase
    {
        [Loggable]
        public Vector2 size;

        public AreaEvent(string name) : base(name)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Entity.AddComponent(new BoxCollider(-size.X/2, -size.Y/2, size.X/2, size.Y/2));
        }
        public override void Draw(Batcher batcher, Vector2 topLeft, Level level)
        {
            base.Draw(batcher, topLeft, level);
            DrawText(batcher, topLeft, level);
        }

        public override Rectangle GetRect(Vector2 topLeft, Level _level)
        {
            return new Rectangle((this.position + topLeft).RoundToPoint(), this.size.RoundToPoint());
        }
    }

    public static class Event
    {
        public static EventBase CoordinateEventFromProto(Levels.Proto.CoordinateEvent e)
        {
            //Log.Information("Parsing coordinate event {@A}", e);
            var pos = new Vector2(e.X, e.Y);
            CoordinateEvent elem = e.Id switch
            {
                Levels.Proto.CoordinateEvent.Types.CoordinateEventId.None => null,
                Levels.Proto.CoordinateEvent.Types.CoordinateEventId.Spawn => new Spawn(e.Data),
                Levels.Proto.CoordinateEvent.Types.CoordinateEventId.HealthPickup => new HealthPickup(),
                Levels.Proto.CoordinateEvent.Types.CoordinateEventId.Enemy => new FloatingBlob(e.Data),
                _ => throw new Exception("Unknown event ID: " + e.Id)
            };
            if (elem != null)
            {
                elem.position = pos;
                Log.Information("Initialized event {A}", elem);
            }
            return elem;
        }

        internal static EventBase AreaEventFromProto(Levels.Proto.AreaEvent e)
        {
            //Log.Information("Parsing area event {@A}", e);
            var pos = new Vector2(e.X, e.Y);
            var size = new Vector2(e.Width, e.Height);
            AreaEvent elem = e.Id switch
            {
                Levels.Proto.AreaEvent.Types.AreaEventId.None => null,
                Levels.Proto.AreaEvent.Types.AreaEventId.ForcedMovement => new ForceMovement(e.Data),
                Levels.Proto.AreaEvent.Types.AreaEventId.LevelEnd => new LevelEnd(e.Data),
                _ => throw new Exception("Unknown event ID: " + e.Id),
            };
            if (elem != null)
            {
                elem.position = pos;
                elem.size = size;
                Log.Information("Initialized areaevent {A}", elem);
            }
            return elem;
        }
    }
}
