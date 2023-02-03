using FGJ23.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;

namespace FGJ23.Components
{
    public class Bone : Component, IUpdatable
    {
        SpriteAnimator _animator;

        private Vector2 _center;
        private Vector2 _dStart;
        private Vector2 _dEnd;
        public Vector2 Size;

        public Vector2 Start => Entity.Transform.Position + _dStart;
        public Vector2 End => Entity.Transform.Position + _dEnd;

        public Bone(Vector2 dStart, Vector2 dEnd)
        {
            this._center = new Vector2(16, 16);
            this._dStart = dStart - _center;
            this._dEnd = dEnd - _center;
            this.Size = dEnd - dStart;
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(FGJ23.Content.Files.Bone);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));

            //// extract the animations from the atlas. they are setup in rows with 8 columns
            _animator.AddAnimation("Default", new[] { sprites[0], });
            _animator.Play("Default");
        }

        public void SetLocation(Vector2 start, Vector2 end)
        {
            var angle = (Vector2.Zero).Angle2(end - start) * Mathf.Deg2Rad;
            Entity.Transform.Position = start + _dEnd.Rotate(angle * Mathf.Rad2Deg);
            Entity.Transform.Rotation = angle;
        }

        void IUpdatable.FixedUpdate() { }

        void IUpdatable.DrawUpdate() { }
    }
}
