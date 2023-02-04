using FGJ23.Components;
using FGJ23.Ext;
using FGJ23.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Nez;
using Nez.UI;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;

namespace FGJ23.Entities
{
    public class Player : Component, ITriggerListener, IUpdatable
    {
        public int ExtraJumps = 1;
        public bool CanShoot = true;
        public bool CanJump = true;
        public bool CanWalljump = true;

        public float JumpHeight = 16 * 6;
        public float WalljumpStrength = 300;
        float jumpLength = 0;
        float wallclimbLength = 0;
        int jumpCount = 1;
        bool cachedJump = false;

        SpriteAnimator _animator;
        //TiledMapMover _mover;
        FGJ23.Levels.Mover _mover;
        BoxCollider _boxCollider;
        readonly Levels.CollisionState _collisionState = new Levels.CollisionState();
        readonly float groundAccel = 800;
        readonly float airAccel = 650;
        public RigidBody _rigidBody;
        int _coyote = 0;

        VirtualButton _jumpInput;
        VirtualButton _fireInput;

        public int Width;
        public int Height;

        string spritepath;
        public Player(int width, int height, string sprites)
        {
            Width = width;
            Height = height;
            spritepath = sprites;
        }

        internal void LockControls(float v)
        {
            ControlsLocked = v;
        }

        VirtualIntegerAxis _xAxisInput;
        Button _leftInput;
        Button _rightInput;

        private float ControlsLocked = 0;
        public bool PreventActions = false;

        public float IgnoreCollisionsFor
        {
            get => _rigidBody.IgnoreCollisionsFor;
        }

        public override void OnAddedToEntity()
        {
            var texture = Entity.Scene.Content.LoadTexture("Content/Files/" + spritepath);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

            _boxCollider = Entity.GetComponent<BoxCollider>();
            _mover = Entity.GetComponent<FGJ23.Levels.Mover>();
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _rigidBody = Entity.AddComponent(new RigidBody(300, groundAccel, airAccel));

            Entity.AddComponent(new Health(5));

            _animator.AddAnimation("Idle", new[] { sprites[0] });
            _animator.AddAnimation("Run", new[] { sprites[0],sprites[1],sprites[2],sprites[3],sprites[4], });
            _animator.AddAnimation("Shooting", new[] { sprites[5], sprites[6] });

            SetupInput();
            ColliderSystem.RegisterCollider(Entity);
        }

        public override void OnRemovedFromEntity()
        {
            // deregister virtual input
            _jumpInput.Deregister();
            _fireInput.Deregister();
            _xAxisInput.Deregister();
            ColliderSystem.UnRegisterCollider(Entity);
        }

        void SetupInput()
        {
            // setup input for jumping. we will allow z on the keyboard or a on the gamepad
            _jumpInput = new VirtualButton();
            _jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            _jumpInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));
            _jumpInput.Nodes.Add(new ButtonNode(Entity.AddComponent(new Button(new RectangleF(200, 225, 50, 50)))));

            _fireInput = new VirtualButton();
            _fireInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
            _fireInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.X));
            _fireInput.Nodes.Add(new ButtonNode(Entity.AddComponent(new Button(new RectangleF(275, 225, 50, 50)))));

            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
            _xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
            _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            _leftInput = Entity.AddComponent(new Button(new RectangleF(0, 225, 50, 50)));
            _rightInput = Entity.AddComponent(new Button(new RectangleF(75, 225, 50, 50)));
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

            if (CanShoot && _fireInput.IsDown)
            {
                animation = "Shooting";
            }

            if (!_animator.IsAnimationActive(animation))
            {
                _animator.Play(animation);
            }
        }

        bool OnSlope()
        {
            return _collisionState.Below &&
                ((Math.Abs(_collisionState.SlopeAngle) >= 40 && _rigidBody.velocity.X > 350) ||
                 (Math.Abs(_collisionState.SlopeAngle) <= -40 && _rigidBody.velocity.X < -350));
        }

        bool CanJumpNow()
        {
            return CanJump && _collisionState.Below || cachedJump || _coyote > 0;
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
                jumpCount = 0;
            }

            if (ControlsLocked == 0 && !PreventActions)
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

                if (OnSlope() && _jumpInput.IsDown)
                {
                    cachedJump = true;
                }
                else if(CanWalljump && !CanJumpNow() && _collisionState.Left && moveDir.X < 0 && _jumpInput.IsPressed) {
                    _rigidBody.velocity.X = WalljumpStrength;
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * WalljumpStrength);
                }
                else if(CanWalljump && !CanJumpNow() && _collisionState.Right && moveDir.X > 0 && _jumpInput.IsPressed) {
                    _rigidBody.velocity.X = -WalljumpStrength;
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * WalljumpStrength);
                }
                else if ((CanJumpNow() && _jumpInput.IsDown) || (jumpCount > 0 && _jumpInput.IsPressed))
                {
                    _coyote = 0;
                    cachedJump = false;
                    if(jumpCount > 0) {
                        jumpCount -= 1;
                    } else {
                        jumpCount = ExtraJumps;
                    }
                    if (_rigidBody.velocityLockedFor > 0)
                    {
                        jumpLength = 0.4f;
                    }
                    else
                    {
                        jumpLength = 0.25f;
                    }
                    wallclimbLength = 0.25f;
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 500);
                }
                else if (CanContinueJump() && _jumpInput.IsDown)
                {
                    _rigidBody.velocity.Y = -Mathf.Sqrt(2f * JumpHeight * 500);
                    jumpLength -= Time.DeltaTime;
                }
                else
                {
                    jumpLength = 0;
                    cachedJump = false;
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
            Vector2 moveDir;
            if (_leftInput.Hits(false) != null)
            {
                moveDir = new Vector2(-1, 0);
            }
            else if (_rightInput.Hits(false) != null)
            {
                moveDir = new Vector2(1, 0);
            }
            else
            {
                moveDir = new Vector2(_xAxisInput.Value, 0);
            }

            UpdateAnimation(moveDir);
            UpdateMovement(moveDir);

            if (!PreventActions && CanShoot && _fireInput.IsPressed)
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

    public class ButtonNode : VirtualButton.Node
    {
        Button button;

        public override bool IsDown => button.Hits(false) != null;

        public override bool IsPressed => button.Hits(true) != null;

        public override bool IsReleased => false;
        public ButtonNode(Button button)
        {
            this.button = button;
        }
    }

    public class Button : RenderableComponent
    {
        RectangleF hit;
        public override RectangleF Bounds => Entity.Scene.Camera.Bounds;

        public Button(RectangleF pos)
        {
            this.hit = pos;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            //batcher.DrawRect(hit.X + Bounds.X, hit.Y + Bounds.Y, hit.Width, hit.Height, Color.DarkGray);
        }

        public Vector2? Hits(bool pressed)
        {
            foreach (var e in Input.Touch.CurrentTouches)
            {
                var mul = new Vector2(Screen.MonitorWidth / Screen.Width, Screen.MonitorHeight / Screen.Height);
                var position = mul * Nez.Input.ScaledPosition(e.Position);

                if (pressed && e.State == TouchLocationState.Pressed && hit.Contains(position))
                {
                    return e.Position;
                }
                if (!pressed && e.State != TouchLocationState.Released && hit.Contains(position))
                {
                    return e.Position;
                }
            }
            if (Input.LeftMouseButtonPressed && hit.Contains(Input.ScaledMousePosition))
            {
                return Input.ScaledMousePosition;
            }
            if (!pressed && Input.LeftMouseButtonDown && hit.Contains(Input.ScaledMousePosition))
            {
                return Input.ScaledMousePosition;
            }
            return null;
        }
    }
}
