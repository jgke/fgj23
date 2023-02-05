using System.Collections.Generic;
using System;
using Nez;
using FGJ23.Support;
using FMOD.Studio;

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
        private static FMOD.Studio.EventInstance foots;
        private static FMOD.Studio.EventInstance selitys;

        private static void DoLoad() {
            if(!musicLoaded) {
                musicLoaded = true;
                evInst = FmodWrapper.GetSound("event:/Pelimusat");
                foots = FmodWrapper.GetSound("event:/Kavely");
                selitys = FmodWrapper.GetSound("event:/Setaselittaa");
                FmodWrapper.HandleError(evInst.start(), "Failed to start audio instance");
            }
        }

        public static void SetMusic(string parameter) {
            DoLoad();

            FmodWrapper.SetParameter("parameter:/Musa1", 0);
            FmodWrapper.SetParameter("parameter:/Musa2", 0);
            FmodWrapper.SetParameter("parameter:/Musa3", 0);
            FmodWrapper.SetParameter("parameter:/Musa4", 0);
            FmodWrapper.SetParameter("parameter:/Musa5", 0);

            if(parameter != "") {
                FmodWrapper.SetParameter("parameter:/" + parameter, 1);
            }
        }



        public static void PlayFoot() {
            DoLoad();
            FmodWrapper.HandleError(foots.getPlaybackState(out var state), "Failed to start audio instance");
            if(state == PLAYBACK_STATE.STOPPED) {
                FmodWrapper.HandleError(foots.start(), "Failed to start audio instance");
            }
        }

        public static void StopFoot() {
            DoLoad();
            FmodWrapper.HandleError(foots.getPlaybackState(out var state), "Failed to start audio instance");
            if(state ==  PLAYBACK_STATE.PLAYING) {
                FmodWrapper.HandleError(foots.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT), "Failed to stop audio instance");
            }
        }

        public static void PlaySeta() {
            DoLoad();
            FmodWrapper.HandleError(selitys.getPlaybackState(out var state), "Failed to start audio instance");
            if(state == PLAYBACK_STATE.STOPPED) {
                FmodWrapper.HandleError(selitys.start(), "Failed to start audio instance");
            }
        }

        public static void StopSeta() {
            DoLoad();
            FmodWrapper.HandleError(selitys.getPlaybackState(out var state), "Failed to start audio instance");
            if(state ==  PLAYBACK_STATE.PLAYING) {
                FmodWrapper.HandleError(selitys.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT), "Failed to stop audio instance");
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
