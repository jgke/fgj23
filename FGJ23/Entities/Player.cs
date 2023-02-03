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
        public float JumpHeight = 16 * 6;
        float jumpLength = 0;
        float wallclimbLength = 0;
        int dashCount = 1;
        bool cachedJump = false;

        SpriteAnimator _animator;
        //TiledMapMover _mover;
        FGJ23.Levels.Mover _mover;
        BoxCollider _boxCollider;
        readonly Levels.CollisionState _collisionState = new Levels.CollisionState();
        readonly float groundAccel = 1000;
        readonly float airAccel = 750;
        RigidBody _rigidBody;
        int _coyote = 0;

        VirtualButton _jumpInput;
        VirtualButton _climbInput;
        VirtualButton _fireInput;

        internal void LockControls(float v)
        {
            ControlsLocked = v;
        }

        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        private float ControlsLocked = 0;

        public float IgnoreCollisionsFor
        {
            get => _rigidBody.IgnoreCollisionsFor;
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture(FGJ23.Content.Files.Player);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<FGJ23.Levels.Mover>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _rigidBody = Entity.AddComponent(new RigidBody(600, groundAccel, airAccel));

            Entity.AddComponent(new Health(5));

            _animator.AddAnimation("Idle", new[] { sprites[0] });
            _animator.AddAnimation("Run", new[] { sprites[1] });
            _animator.AddAnimation("Falling", new[] { sprites[2] });
            _animator.AddAnimation("Jumping", new[] { sprites[3] });

            SetupInput();
            ColliderSystem.RegisterCollider(Entity);
        }

        public override void OnRemovedFromEntity()
        {
            // deregister virtual input
            _jumpInput.Deregister();
            _fireInput.Deregister();
            _climbInput.Deregister();
            _xAxisInput.Deregister();
            _yAxisInput.Deregister();
            ColliderSystem.UnRegisterCollider(Entity);
        }

        void SetupInput()
        {
            // setup input for jumping. we will allow z on the keyboard or a on the gamepad
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            _jumpInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            _fireInput = new VirtualButton();
            _fireInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
            _fireInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.X));

            // setup input for climb
            _climbInput = new VirtualButton();
            _climbInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.O));
            _climbInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.B));

            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            // horizontal input from dpad, left stick or keyboard left/right
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
                if (_collisionState.Below)
                {
                    animation = "Run";
                }
                _animator.FlipX = true;
            }
            else if (moveDir.X > 0)
            {
                if (_collisionState.Below)
                {
                    animation = "Run";
                }
                _animator.FlipX = false;
            }

            if (_jumpInput.IsDown && jumpLength > 0)
            {
                animation = "Jumping";
            }

            if (!_collisionState.Below && _rigidBody.velocity.Y > 0)
            {
                animation = "Falling";
            }

            if (!_animator.IsAnimationActive(animation))
            {
                _animator.Play(animation);
            }
        }

        bool CanDash()
        {
            return !_collisionState.Below && dashCount > 0;
        }

        bool OnSlope()
        {
            return _collisionState.Below &&
                ((Math.Abs(_collisionState.SlopeAngle) >= 40 && _rigidBody.velocity.X > 350) ||
                 (Math.Abs(_collisionState.SlopeAngle) <= -40 && _rigidBody.velocity.X < -350));
        }

        bool CanJump()
        {
            return _collisionState.Below || cachedJump || _coyote > 0;
        }

        bool CanContinueJump()
        {
            return jumpLength > 0 && !_collisionState.Above;
        }

        void UpdateCoyote()
        {
            if (!_collisionState.Below && _coyote > 0)
            {
                _coyote -= 1;
            }
            else if (_collisionState.Below)
            {
                _coyote = 10;
            }
        }

        void UpdateMovement(Vector2 moveDir)
        {
            UpdateCoyote();
            if (_collisionState.Below)
            {
                dashCount = 1;
            }

            if (ControlsLocked == 0)
            {
                if (moveDir.X < 0)
                {
                    _rigidBody.AccelLeft(_collisionState);
                }
                else if (moveDir.X > 0)
                {
                    _rigidBody.AccelRight(_collisionState);
                }
                else
                {
                    _rigidBody.SlowDown(_collisionState);
                }

                if (CanDash() && moveDir != Vector2.Zero && _jumpInput.IsPressed)
                {
                    dashCount -= 1;
                    var direction = Vector2.Normalize(new Vector2(Math.Sign(moveDir.X), Math.Sign(moveDir.Y)));
                    var speed = _rigidBody.velocity.Length();
                    if (Math.Abs(speed) < 400)
                    {
                        speed = Math.Sign(speed) * 400;
                    }
                    _rigidBody.velocity = direction * speed;

                }
                else if (OnSlope() && _jumpInput.IsDown)
                {
                    cachedJump = true;
                }
                else if (CanJump() && _jumpInput.IsDown)
                {
                    _coyote = 0;
                    cachedJump = false;
                    dashCount = 1;
                    if (_rigidBody.velocityLockedFor > 0)
                    {
                        jumpLength = 0.4f;
                    }
                    else
                    {
                        jumpLength = 0.25f;
                    }
                    wallclimbLength = 0.25f;
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 1000);
                }
                else if (CanContinueJump() && _jumpInput.IsDown)
                {
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 1000);
                    jumpLength -= Time.DeltaTime;
                }
                else
                {
                    jumpLength = 0;
                    cachedJump = false;
                }

                if ((_collisionState.Left && moveDir.X < 0) || (_collisionState.Right && moveDir.X > 0))
                {
                    if (_rigidBody.velocity.Y < 0 && wallclimbLength > 0 && jumpLength <= 0 && _climbInput.IsDown && (_jumpInput.IsDown || _yAxisInput < 0))
                    {
                        //_rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 1000);
                        //_rigidBody.gravity = 1000;
                        _rigidBody.gravity = 0;
                        wallclimbLength -= Time.DeltaTime;
                    }
                    else if (_climbInput.IsDown)
                    {
                        //_rigidBody.velocity.Y = Math.Min(_rigidBody.velocity.Y, 50);
                        if (_rigidBody.velocity.Y > 0)
                        {
                            _rigidBody.gravity = 100;
                        }
                        else
                        {
                            _rigidBody.gravity = 1000;
                        }
                    }
                }
                else
                {
                    _rigidBody.gravity = 1000;
                }
            }
            else
            {
                ControlsLocked -= Time.DeltaTime;
                if (ControlsLocked <= 0)
                {
                    ControlsLocked = 0;
                }
            }

            if (_rigidBody.IgnoreCollisionsFor > 0)
            {
                _rigidBody.IgnoreCollisionsFor -= Time.DeltaTime;
                ColliderSystem.ForceMove(_boxCollider, _rigidBody.velocity * Time.DeltaTime);
                if (_rigidBody.IgnoreCollisionsFor <= 0)
                {
                    _rigidBody.IgnoreCollisionsFor = 0;
                    ForceMovementSystem.StopForceMove(this);
                }
            }
            else
            {
                _mover.Move(_rigidBody.velocity * Time.DeltaTime, _boxCollider, _collisionState);
                _rigidBody.UpdateCollisions(_collisionState);
            }
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
                bullet.AddComponent(_mover);
            }

            //Console.WriteLine("Phys {0} {1}",
            //        Entity.PreviousTransform.Position,
            //        Entity.Transform.Position);

        }

        void IUpdatable.DrawUpdate()
        {
            // handle movement and animations
            //Console.WriteLine(
            //        "Draw {0} {1} {2} {3}",
            //        Entity.PreviousTransform.Position,
            //        _animator.GraphicsTransform.Position,
            //        Entity.Transform.Position,
            //        Time.Alpha
            //        );
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
