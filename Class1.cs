using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using BepInEx;
using IL.Menu;
using Menu;
using Menu.Remix.MixedUI;
using More_title_screens;
using System.Diagnostics;

namespace MoreTitleScreens
{

    [BepInPlugin("Blujai.Titles", "More Titles Screens", "1.0")]
    public class TitleScreens : BaseUnityPlugin
    {
        public const string MOD_ID = "Blujai.Titles";
        private string customTitlesFilePath;
        private string customTitlesDirectory;
        public static bool IsPostInit;
 
        public void OnEnable()
        {
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;

        }
        
        public  void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            if (IsPostInit)
            {
                return;
            }
            IsPostInit = true;
            string modDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string gameDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            customTitlesFilePath = AssetManager.ResolveFilePath("customTitles.txt");
            
            customTitlesDirectory = Path.GetDirectoryName(customTitlesFilePath) + "/illustrations";
            Logger.LogInfo($"Resolved custom titles file path: {customTitlesFilePath}");
            Logger.LogInfo($"Custom titles directory: {customTitlesDirectory}");

            if (MachineConnector.GetRegisteredOI(MOD_ID) != MTSconfig.Instance)
                MachineConnector.SetRegisteredOI(MOD_ID, MTSconfig.Instance);

            IL.Menu.IntroRoll.ctor += IntroRoll_ctor;
        }
        
        public void IntroRoll_ctor(ILContext il)
        {
            var cursor = new ILCursor(il);
            

            try
            {
                if (cursor.TryGotoNext(i => i.MatchNewarr<string>()))
                {
                    int localIndex = -1;

                    if (cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(out localIndex)))
                    {
                        cursor.Emit(OpCodes.Ldloc, localIndex); // Load the local variable
                        cursor.EmitDelegate<Func<string[], string[]>>((oldTitleImages) =>
                        {
                            if (MTSconfig.OnlyCustomTitles.Value)
                            {
                                var unwantedTitles = new HashSet<string> { "gourmand", "spear", "rivulet", "saint", "arti" };
                                oldTitleImages = oldTitleImages.Where(title => !unwantedTitles.Contains(title)).ToArray();
                                Logger.LogMessage("only custom screens");
                            }
                            else
                            {
                                Logger.LogMessage("should have msc title screens too");
                            }
                            string[] customTitles = new string[0];
                            try
                            {
                                if (File.Exists(customTitlesFilePath))
                                {
                                    customTitles = File.ReadAllLines(customTitlesFilePath);
                                    Logger.LogMessage($"read custom titles: {string.Join(", ", customTitles)} ");
                                }
                                else
                                {
                                    Logger.LogWarning($"Custom titles file not found: {customTitlesFilePath}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Error reading custom titles file: {ex.Message}");
                            }

                            return oldTitleImages.Concat(customTitles).ToArray();
                        });
                        cursor.Emit(OpCodes.Stloc, localIndex); // Store the modified array back into the local variable
                    }
                    else
                    {
                        Logger.LogError("Stloc instruction not found.");
                    }
                }
                else
                {
                    Logger.LogMessage("Newarr instruction not found.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception occurred in IL editing: {ex.Message}");
                Logger.LogError(ex.StackTrace);
            }
        }
        
    }
  
}
