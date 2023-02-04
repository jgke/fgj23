using Nez.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Tiled;
using Nez.Textures;
using Nez.UI;
using System;
using System.IO;
using FGJ23.Core;
using FGJ23.Support;

namespace FGJ23
{

    public class TouchableButton : TextButton {
        public class TouchableComponent : Component, IUpdatable {
            TouchableButton parent;
            FGJ23.Entities.Button hitTester;
            public TouchableComponent(TouchableButton parent) {
                this.parent = parent;
            }

            void IUpdatable.FixedUpdate() {
                hitTester = new FGJ23.Entities.Button(new RectangleF(parent.x, parent.y, parent.width, parent.height));
                var mhit = hitTester.Hits(true);
                if(mhit is {} hit) {
                    ((IInputListener)parent).OnLeftMousePressed(hit);
                }
            }
            void IUpdatable.DrawUpdate() { }
        }


        public TouchableButton(string label, TextButtonStyle style): base(label, style) {

        }

        public TouchableComponent ToTouchableComponent() {
            return new TouchableComponent(this);
        }
    }

    class MenuComponent : Component
    {
        public override void OnAddedToEntity()
        {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            var titleLabel = new Label("FGJ 23", new LabelStyle()
            {
                FontColor = Color.Black,
                FontScaleX = 3,
                FontScaleY = 3,
            });
            table.Add(titleLabel);
            table.Row();

            var button1 = new TouchableButton("Start game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            Entity.AddComponent(button1.ToTouchableComponent());
            table.Add(button1).SetMinWidth(100).SetMinHeight(30).SetSpaceBottom(10);
            table.Row();
            button1.OnClicked += _ =>
            {
                GameState.Instance.DoTransition(() => new GameplayScene(null));
            };
            var button2 = new TouchableButton("Quit game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            Entity.AddComponent(button2.ToTouchableComponent());
            table.Add(button2).SetMinWidth(100).SetMinHeight(30);
            table.Row();
            button2.OnClicked += _ =>
            {
                Nez.Core.Exit();
            };
        }
    }

    public class MenuScene : SceneBase
    {
        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("menu", new Vector2(0, 0));
            menuEntity.AddComponent(new MenuComponent());

            Log.Information("Initializing new MenuScene");
            int x = 800;
            int y = 600;
            SetDesignResolution(x, y, SceneResolutionPolicy.ShowAllPixelPerfect);
            if (OperatingSystem.IsAndroid())
            {
                    Nez.Screen.IsFullscreen = true;
                    Nez.Screen.SetSize(
                            Screen.MonitorWidth,
                            Screen.MonitorHeight
                    );
                    Nez.Screen.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            } else {
                Screen.SetSize(x * 2, y * 2);
            }
        }
    }
}
