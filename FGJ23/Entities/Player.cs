using FGJ23.Components;
using FGJ23.Ext;
using FGJ23.Systems;
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
    public class Player : Component, ITriggerListener, IUpdatable
    {
        SpriteAnimator _animator;
        RigidBody _rigidBody;

        VirtualButton _fireInput;
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(FGJ23.Content.Files.Player);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _rigidBody = Entity.AddComponent(new RigidBody(600, 10, 10));

            _animator.AddAnimation("Idle", new[] { sprites[0] });
            _animator.AddAnimation("Run", new[] { sprites[1] });
            _animator.AddAnimation("Falling", new[] { sprites[2] });
            _animator.AddAnimation("Jumping", new[] { sprites[3] });

            SetupInput();
            ColliderSystem.RegisterCollider(Entity);
        }

        public override void OnRemovedFromEntity()
        {
            _fireInput.Deregister();
            _xAxisInput.Deregister();
            _yAxisInput.Deregister();
            ColliderSystem.UnRegisterCollider(Entity);
        }

        void SetupInput()
        {
            _fireInput = new VirtualButton();
            _fireInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
            _fireInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.X));

            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
            _yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
            _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));
        }

        void UpdateAnimation(Vector2 moveDir)
        {
            string animation = "Idle";
            if (moveDir.X < 0)
            {
                _animator.FlipX = true;
            }
            else if (moveDir.X > 0)
            {
                _animator.FlipX = false;
            }

            if (!_animator.IsAnimationActive(animation))
            {
                _animator.Play(animation);
            }
        }


        void UpdateMovement(Vector2 moveDir)
        {

        }

        void IUpdatable.FixedUpdate()
        {
            // handle movement and animations
            var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);

            UpdateAnimation(moveDir);
            UpdateMovement(moveDir);

            if (_fireInput.IsPressed)
            {
                var bullet = Entity.Scene.CreateEntity("bullet", Transform.Position);
                bullet.AddComponent(new Bullet(BulletType.Basic, _animator.FlipX));
                bullet.AddComponent(new BoxCollider(-4, -4, 4, 4));
                bullet.AddComponent(new Damage(1, this.Entity));
            }
        }

        void IUpdatable.DrawUpdate()
        {

        }

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
