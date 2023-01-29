using Microsoft.Xna.Framework;
using Nez;

namespace FGJ23.Components
{
    public class DrawRectangle : RenderableComponent
    {
        int width;
        int height;
        Color color;

        public override float Width => width;
        public override float Height => height;

        public DrawRectangle(int width, int height, Color color)
        {
            this.width = width;
            this.height = height;
            this.color = color;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            batcher.DrawRect(Entity.Transform.Position.X, Entity.Transform.Position.Y, width, height, color);
        }
    }
}
