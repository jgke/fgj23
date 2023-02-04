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

            var button1 = new TextButton("Start game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button1).SetMinWidth(100).SetMinHeight(30).SetSpaceBottom(10);
            table.Row();
            button1.OnClicked += _ =>
            {
                GameState.Instance.DoTransition(() => new GameplayScene(null));
            };
            var button2 = new TextButton("Quit game", TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
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
        }
    }
}
