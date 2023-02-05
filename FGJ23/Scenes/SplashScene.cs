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
using FGJ23.Levels;

namespace FGJ23
{
    class SplashSceneComponent : Component
    {
        public override void OnAddedToEntity()
        {
            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            var texture = Entity.Scene.Content.LoadTexture("Content/Files/fmod-logo.png");

            var table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);
            table.SetBackground(new PrimitiveDrawable(Color.Black));

            var fmodLogo = new Image(texture);
            table.Add(fmodLogo).Fill();
            table.Row();
        }
    }

    public class SplashScene : SceneBase
    {
        int ticks = 140;

        public override void Initialize()
        {
            base.Initialize();

            var menuEntity = CreateEntity("fmodlogo", new Vector2(0, 0));
            menuEntity.AddComponent(new SplashSceneComponent());

            Log.Information("Initializing new SplashScene");
            int x = 400;
            int y = 300;
            SetDesignResolution(x, y, SceneResolutionPolicy.ShowAllPixelPerfect);
            if (OperatingSystem.IsAndroid())
            {
                Nez.Screen.IsFullscreen = true;
                Nez.Screen.SetSize(
                        Screen.MonitorWidth,
                        Screen.MonitorHeight
                );
                Nez.Screen.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            }
            else
            {
                Screen.SetSize(x * 2, y * 2);
            }
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
            ticks -= 1;
            if(ticks < 0 && !GameState.Instance.Transitioning) {
                GameState.Instance.DoTransition(() => new MenuScene());
            }
        }
    }
}
