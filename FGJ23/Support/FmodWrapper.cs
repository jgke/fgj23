using FMOD.Studio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FGJ23.Core;
using System.Net;

#if ANDROID
using Android.Content;
using Java.Nio.Channels;
using Android.App;
#endif

namespace FGJ23.Support
{
    enum Receiver { Val };
    public class FmodWrapper
    {
        private static bool loaded = false;
        private static FMOD.Studio.System studioSystem;
        private static FMOD.System coreSystem;
            
		private static List<Bank> _banks = new List<Bank>();

#if !ANDROID
        string masterBankLocation = "Content/fmod/Master.bank";
        string masterStringBankLocation = "Content/fmod/Master.strings.bank";
#else
        string masterBankLocation = "Audio/audio/Build/Mobile/Master.bank";
        string masterStringBankLocation = "Audio/audio/Build/Mobile/Master.strings.bank";
#endif

        public void Init()
        {
            if (loaded)
            {
                Log.Fatal("Already loaded FMOD once, bailing!");
                throw new Exception();
            }
            loaded = true;

#if !ANDROID
            NativeLibrary.SetDllImportResolver(
                Receiver.Val.GetType().Assembly,
                (libraryName, assembly, dllImportSearchPath) =>
                {
                    libraryName = Path.GetFileNameWithoutExtension(libraryName);
                    if (dllImportSearchPath == null)
                    {
                        dllImportSearchPath = DllImportSearchPath.AssemblyDirectory;
                    }
                    return NativeLibrary.Load(LibraryName(libraryName, true), assembly, dllImportSearchPath);
                }
            );
#else
            Java.Lang.JavaSystem.LoadLibrary(LibraryName("fmod", false));
            Java.Lang.JavaSystem.LoadLibrary(LibraryName("fmodstudio", false));
#endif

            HandleError(FMOD.Memory.GetStats(out var currentallocated, out var maxallocated), "Failed to initialize FMOD Core (GetStats)");

            HandleError(FMOD.Studio.System.create(out studioSystem), "Failed to load FMOD Studio");
            HandleError(studioSystem.getCoreSystem(out coreSystem), "Failed to load FMOD Core");

            var maxChannels = 256;
            FMOD.INITFLAGS coreInitFlags = FMOD.INITFLAGS.CHANNEL_LOWPASS | FMOD.INITFLAGS.CHANNEL_DISTANCEFILTER;
            FMOD.Studio.INITFLAGS studioInitFlags = FMOD.Studio.INITFLAGS.NORMAL | FMOD.Studio.INITFLAGS.LIVEUPDATE;

            uint dspBufferLength = 4;
            int dspBufferCount = 32;

            HandleError(studioSystem.initialize(maxChannels, studioInitFlags, coreInitFlags, (IntPtr)0), "Failed to load FMOD Studio");

            coreSystem.setDSPBufferSize(dspBufferLength, dspBufferCount);

            var masterData = ResourceLoader.ReadResource(masterBankLocation);
            HandleError(studioSystem.loadBankMemory(masterData, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out var bank), "Failed to load master bank");
            _banks.Add(bank);
            var stringData = ResourceLoader.ReadResource(masterStringBankLocation);
            HandleError(studioSystem.loadBankMemory(stringData, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out bank), "Failed to load master strings bank");
            _banks.Add(bank);

            HandleError(_banks[0].loadSampleData(), "Failed to load master bank samples");
            HandleError(_banks[1].loadSampleData(), "Failed to load master bank samples");

            Log.Information("Loaded FMOD!");
        }

        public static void SetParameter(string paramId, float value)
        {
            HandleError(studioSystem.setParameterByName(paramId, value), "Failed to set parameter " + paramId + " to value " + value);
        }

        public static void HandleError(FMOD.RESULT result, String msg)
        {
            if (result != FMOD.RESULT.OK)
            {
                Log.Fatal(msg + ": {A}", result);
                throw new Exception(msg + ": "+ result);
            }
        }

        private static string LibraryName(string libName, bool loggingEnabled = false)
        {
            if (OperatingSystem.IsWindows())
            {
                return loggingEnabled ? $"{libName}L.dll" : $"{libName}.dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                return loggingEnabled ? $"{libName}L.so" : $"{libName}.so";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return loggingEnabled ? $"{libName}L.dylib" : $"{libName}.dylib";
            }
            else if (OperatingSystem.IsAndroid())
            {
                return $"{libName}"; // FIXME: debug-libraries are not loading (INTERNAL_ERROR on creating studio system)
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static EventInstance GetSound(String path)
        {
            HandleError(studioSystem.getEvent(path, out var ev), "Failed to load audio event " + path);
            HandleError(ev.createInstance(out var evInst), "Failed to create audio instance for event " + path);
            return evInst;
        }

        public static void PlaySound(String path)
        {
            var evInst = GetSound(path);
            HandleError(evInst.start(), "Failed to start audio instance for event " + path);
            HandleError(evInst.release(), "Failed to release audio instance for event " + path);
        }

        public static void Unload()
        {
            foreach(var bank in _banks)
            {
                bank.unload();
            }
            studioSystem.release();
        }

        public static void FixedUpdate(GameTime _gameTime)
        {
            HandleError(studioSystem.update(), "FMOD update failed");
        }
    }
}
