using FGJ23.Components;
using FGJ23.Entities.Enemies;
using FGJ23.Support;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;

namespace FGJ23.Entities
{
    public enum BulletType
    {
        Basic = 0,
    }

    public class Bullet : Component, ITriggerListener, IUpdatable
    {
        SpriteAnimator _animator;
        BoxCollider _boxCollider;
        RigidBody _rigidBody;
        int frames = 300;
        string animation = "Fly";

        BulletType type;
        Vector2 velocity;

        public Bullet(BulletType type, bool goLeft)
        {
            this.type = type;
            if (goLeft)
            {
                velocity = new Vector2(-1000, 0);
            }
            else
            {
                velocity = new Vector2(1000, 0);
            }
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(FGJ23.Content.Files.Bullet);
            var sprites = Sprite.SpritesFromAtlas(texture, 8, 8);

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _rigidBody = Entity.AddComponent(new RigidBody(1000, 1000, 1000));
            _rigidBody.gravity = 0;
            _rigidBody.velocity = velocity;

            var ty = (int)type;

            // extract the animations from the atlas. they are setup in rows with 8 columns
            _animator.AddAnimation("Fly", new[]
            {
                sprites[ty*8 + 0],
                sprites[ty*8 + 1],
                sprites[ty*8 + 2],
                sprites[ty*8 + 3],
            });

            _animator.AddAnimation("Die", new[]
            {
                sprites[ty*8 + 4 + 0],
                sprites[ty*8 + 4 + 1],
                sprites[ty*8 + 4 + 2],
                sprites[ty*8 + 4 + 3],
            });


            Entity.AddComponent(new CollideWithEnemy(this.OnCollide));

            var evInst = FmodWrapper.GetSound("event:/shoot_1");
            FmodWrapper.HandleError(evInst.start(), "Failed to start audio instance");
            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/gun_pitch", Nez.Random.NextFloat(1)), "Failed to update parameter");
            FmodWrapper.HandleError(evInst.release(), "Failed to release audio instance");
        }

        private bool OnCollide(Enemy enemy, bool hadContactOnPreviousFrame)
        {
            enemy.GetComponent<Health>().Hit(1);
            return false;
        }

        void UpdateAnimation()
        {
            if (!_animator.IsAnimationActive(animation))
            {
                if (animation == "Die")
                {
                    _animator.Play(animation, SpriteAnimator.LoopMode.ClampForever);
                }
                else
                {
                    _animator.Play(animation);
                }
            }
        }

        void IUpdatable.FixedUpdate()
        {
            if (frames == -2)
            {
                return;
            }
            else if (frames == -1)
            {
                this.frames = -2;
                this.Entity.Destroy();
                return;
            }
            frames -= 1;

            UpdateAnimation();
        }

        void IUpdatable.DrawUpdate() { }

        #region ITriggerListener implementation

        void ITriggerListener.OnTriggerEnter(Collider other, Collider self)
        {
            Debug.Log("triggerEnter: {0}", other.Entity.Name);
        }

        void ITriggerListener.OnTriggerExit(Collider other, Collider self)
        {
            Debug.Log("triggerExit: {0}", other.Entity.Name);
        }

        #endregion
    }
}
