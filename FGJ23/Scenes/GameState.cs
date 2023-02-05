using System.Collections.Generic;
using System;
using Nez;
using FGJ23.Support;

namespace FGJ23
{
    public class GameState : ILoggable
    {
        static GameState _instance;

        public static GameState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameState(0);
                }
                return _instance;
            }
        }

        private static bool musicLoaded = false;
        private static FMOD.Studio.EventInstance evInst;

        public static void SetMusic(string parameter) {
            if(!musicLoaded) {
                musicLoaded = true;
                evInst = FmodWrapper.GetSound("event:/Pelimusat");
                FmodWrapper.HandleError(evInst.start(), "Failed to start audio instance");
            }

            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/Musa1", 0), "Failed to update parameter");
            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/Musa2", 0), "Failed to update parameter");
            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/Musa3", 0), "Failed to update parameter");
            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/Musa4", 0), "Failed to update parameter");
            FmodWrapper.HandleError(evInst.setParameterByName("parameter:/Musa5", 0), "Failed to update parameter");

            if(parameter != "") {
                FmodWrapper.HandleError(evInst.setParameterByName("parameter/:" + parameter, 1), "Failed to update parameter");
            }
        }

        [Loggable]
        public int LevelNum;
        [Loggable]
        public int PlayerSpeed = 150;
        [Loggable]
        public int Counter = 0;
        [Loggable]
        public HashSet<string> Upgrades = new HashSet<string>();
        [Loggable]
        public bool Transitioning;

        public GameState(int levelNum)
        {
            LevelNum = levelNum;
        }

        public void DoTransition(Func<Scene> act)
        {
            this.Transitioning = true;
            Nez.Core.StartSceneTransition(new WindTransition(act)).OnTransitionCompleted += () =>
            {
                this.Transitioning = false;
            };
        }

        public static Action OnStoryComplete;

        public void StoryComplete()
        {
            if (OnStoryComplete != null)
            {
                OnStoryComplete();
                OnStoryComplete = null;
            }
        }
    }
}
