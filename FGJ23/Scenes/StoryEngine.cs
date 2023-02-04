using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using FGJ23.Components;
using Nez.Tiled;
using Nez.Textures;
using FGJ23.Systems;
using Nez.UI;
using System.Collections.Generic;
using Serilog;
using System;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace FGJ23.Core
{
    public interface StoryPiece
    {
        void CreateUI(Table table, Entity entity, Action cycleStory);
        void DoCycle(Action cycleStory)
        {
            cycleStory();
        }
    }

    public static class UiComponents
    {
        public static TextButton WrappingTextButton(string Text, Action act)
        {
            var style = TextButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green);
            var button1 = new TextButton(Text, style);
            button1.GetLabel().SetWrap(true);
            button1.PadLeft(5);
            button1.PadRight(5);

            button1.OnClicked += _ =>
            {
                if (!GameState.Instance.Transitioning)
                {
                    act();
                }
            };
            return button1;
        }

        public static string SafeString(string inputString)
        {
            return Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback("_"),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(inputString)
                ));
        }
    }

    public class Line : StoryPiece
    {
        public string Avatar;
        public string Character;
        public string Text;
        public bool CharacterIsRight;

        public Line(string avatar, string character, string text, bool characteIsRight)
        {
            this.Avatar = avatar;
            this.Character = UiComponents.SafeString(character);
            this.Text = UiComponents.SafeString(text);
            this.CharacterIsRight = characteIsRight;
        }

        public void CreateUI(Table mainTable, Entity entity, Action cycleStory)
        {
            Log.Information("Line {@A}", this);
            mainTable.Bottom();
            var table = new Table();
            mainTable.Add(table).Expand().Bottom().SetFillX();

            LabelStyle labelStyle = new LabelStyle()
            {
                FontColor = Color.Black,
                Background = new PrimitiveDrawable(Color.DarkGray)
            };

            if (this.CharacterIsRight)
            {
                var fakeTitle = new Label(" ", labelStyle);
                table.Add(fakeTitle).Expand().Bottom().SetFillX();

                var title = new Label(Character, labelStyle);
                title.SetAlignment(Align.Right, Align.Bottom);
                table.Add(title).Bottom().Width(64);
                table.Row();

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(64).Expand().Bottom().SetFillX();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(64).Height(64);
            }
            else
            {
                var title = new Label(Character, labelStyle);
                title.SetAlignment(Align.Left, Align.Bottom);
                table.Add(title).Bottom().Width(64);
                var fakeTitle = new Label(" ", labelStyle);
                table.Add(fakeTitle).Expand().Bottom().SetFillX();
                table.Row();

                var img = new Image(entity.Scene.Content.LoadTexture("Content/" + Avatar));
                table.Add(img).Bottom().Width(64).Height(64);

                var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
                table.Add(button1).SetMinHeight(64).Expand().Bottom().SetFillX();
            }
        }
    }

    public class IncrementCounterBy : StoryPiece
    {
        public int Amount;

        public IncrementCounterBy(int amount)
        {
            this.Amount = amount;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("IncrementCounterBy {@A}", this);
            DoCycle(cycleStory);
        }

        public void DoCycle(Action cycleStory)
        {
            GameState.Instance.Counter += Amount;
            cycleStory();
        }
    }

    public class Exposition : StoryPiece
    {
        public string Text;

        public Exposition(string text)
        {
            this.Text = UiComponents.SafeString(text);
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("Exposition {@A}", this);
            table.Bottom();
            var button1 = UiComponents.WrappingTextButton(Text, cycleStory);
            table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
        }
    }

    public class Fork : StoryPiece
    {
        public int? choice = null;
        List<string> Choices;
        List<StoryBuilder> ChoiceBuilders;

        public Fork(List<string> choices, List<StoryBuilder> choiceBuilders)
        {
            Choices = choices;
            ChoiceBuilders = choiceBuilders;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("Fork {@A}", this);
            if (choice == null)
            {
                Log.Information("Fork choice null");
                table.Bottom();
                for (int i = 0; i < Choices.Count; i++)
                {
                    int num = i;
                    var button1 = UiComponents.WrappingTextButton(UiComponents.SafeString(Choices[i]), () =>
                    {
                        table.Clear();
                        choice = num;
                        this.CreateUI(table, entity, cycleStory);
                    });
                    table.Add(button1).SetMinHeight(100).Expand().Bottom().SetFillX();
                }
            }
            else
            {
                Log.Information("Fork choice {@A}", choice);

                ChoiceBuilders[choice.Value].CreateUI(table, entity, cycleStory);
            }
        }
    }

    public class CounterFork : StoryPiece
    {
        List<Func<int, bool>> Conditions;
        List<StoryBuilder> Builders;
        StoryBuilder currentStory = null;

        public CounterFork(List<Func<int, bool>> conditions, List<StoryBuilder> builders)
        {
            Conditions = conditions;
            Builders = builders;
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            if (currentStory == null)
            {
                Log.Information("CounterFork {@A}", this);
                for (int i = 0; i < Conditions.Count; i++)
                {
                    if (Conditions[i](GameState.Instance.Counter))
                    {
                        currentStory = Builders[i];
                        break;
                    }
                }

                currentStory.CreateUI(table, entity, cycleStory);
            }
        }
    }

    public class GoToLevel : StoryPiece
    {
        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("GoToLevel {@A}", this);
            GameState.Instance.StoryComplete();
        }
    }

    public class ForkBuilder
    {
        public List<string> Choices;
        public List<StoryBuilder> ChoiceBuilders;

        public ForkBuilder()
        {
            Choices = new List<string>();
            ChoiceBuilders = new List<StoryBuilder>();
        }
        public ForkBuilder Choice(string text, StoryBuilder builder)
        {
            Choices.Add(text);
            ChoiceBuilders.Add(builder);
            return this;
        }
    }

    public class CounterForkBuilder
    {
        public List<Func<int, bool>> Conditions;
        public List<StoryBuilder> Builders;
        public CounterForkBuilder()
        {
            Conditions = new List<Func<int, bool>>();
            Builders = new List<StoryBuilder>();
        }
        public CounterForkBuilder IfMoreThan(int amount, StoryBuilder innerContent)
        {
            Conditions.Add(n => n > amount);
            Builders.Add(innerContent);
            return this;
        }
        public CounterForkBuilder Otherwise(StoryBuilder innerContent)
        {
            Conditions.Add(n => true);
            Builders.Add(innerContent);
            return this;
        }
    }

    public class StoryBuilder : StoryPiece
    {
        public List<StoryPiece> lines;
        int currentStoryLine = 0;

        public StoryBuilder()
        {
            lines = new List<StoryPiece>();
        }

        public StoryBuilder Line(string avatar, string by, string what)
        {
            lines.Add(new Line(avatar, by, what, false));
            return this;
        }

        public StoryBuilder LineRight(string avatar, string by, string what)
        {
            lines.Add(new Line(avatar, by, what, true));
            return this;
        }

        public StoryBuilder IncrementCounterBy(int amount)
        {
            lines.Add(new IncrementCounterBy(amount));
            return this;
        }

        public StoryBuilder Exposition(string text)
        {
            lines.Add(new Exposition(text));
            return this;
        }

        public StoryBuilder Fork(ForkBuilder builder)
        {
            lines.Add(new Fork(builder.Choices, builder.ChoiceBuilders));
            return this;
        }

        public StoryBuilder CounterFork(CounterForkBuilder builder)
        {
            lines.Add(new CounterFork(builder.Conditions, builder.Builders));
            return this;
        }

        public StoryComponent GoToLevel()
        {
            lines.Add(new GoToLevel());
            return new StoryComponent(this);
        }

        public void CreateUI(Table table, Entity entity, Action cycleStory)
        {
            Log.Information("StoryBuilder choice={@A} {@B}", currentStoryLine, this);
            if (currentStoryLine < lines.Count)
            {
                var line = lines[currentStoryLine];
                line.CreateUI(table, entity, () =>
                {
                    if (currentStoryLine < lines.Count - 1)
                    {
                        currentStoryLine += 1;
                        table.ClearChildren();
                        this.CreateUI(table, entity, cycleStory);
                    }
                    else
                    {
                        cycleStory();
                    }
                });
            }
            else
            {
                cycleStory();
            }
        }
    }

    public class StoryComponent : Component, IUpdatable
    {
        StoryBuilder storyBuilder;
        Table table;
        private VirtualButton StoryAdvanceButton;

        public StoryComponent(StoryBuilder builder)
        {
            this.storyBuilder = builder;
        }

        public override void OnAddedToEntity()
        {
            StoryAdvanceButton = new VirtualButton();
            StoryAdvanceButton.Nodes.Add(new VirtualButton.KeyboardKey(Keys.A));
            StoryAdvanceButton.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

            Log.Information("Loading story {A}", GameState.Instance);

            UICanvas canvas = new UICanvas();
            Entity.AddComponent(canvas);

            table = canvas.Stage.AddElement(new Table());
            table.SetFillParent(true);

            RenderUI();
        }

        public override void OnRemovedFromEntity()
        {
            StoryAdvanceButton.Deregister();
        }

        public void RenderUI()
        {
            table.ClearChildren();
            storyBuilder.CreateUI(table, Entity, () =>
            {
                throw new Exception("unreachable");
            });
        }



        void IUpdatable.DrawUpdate() { }
        void IUpdatable.FixedUpdate()
        {
            if (StoryAdvanceButton.IsPressed && !GameState.Instance.Transitioning)
            {
                RenderUI();
            }
        }
    }
}
