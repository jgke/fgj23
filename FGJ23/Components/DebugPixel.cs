using Microsoft.Xna.Framework;
using Nez;

namespace FGJ23.Components
{
    public class DebugPixel : Component
    {
        public override void DebugRender(Batcher batcher)
        {
            batcher.DrawPixel(Entity.Transform.Position.X, Entity.Transform.Position.Y, Debug.Colors.DebugText, 3);
        }
    }
}
