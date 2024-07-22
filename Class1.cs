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

namespace MoreTitleScreens
{
    [BepInPlugin("Blujai.Titles", "More Titles Screens", "1.0")]
    public class TitleScreens : BaseUnityPlugin
    {
        private string customTitlesFilePath;
        private string customTitlesDirectory;
        public static bool IsPostInit;

        public void OnEnable()
        {
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        }

        public void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            if (IsPostInit)
            {
                return;
            }
            IsPostInit = true;
            customTitlesFilePath = AssetManager.ResolveFilePath("customTitles.txt");
            customTitlesDirectory = Path.GetDirectoryName(customTitlesFilePath) + "/illustrations";
            Logger.LogInfo($"Resolved custom titles file path: {customTitlesFilePath}");
            Logger.LogInfo($"Custom titles directory: {customTitlesDirectory}");
            IL.Menu.IntroRoll.ctor += IntroRoll_ctor;
        }

        public void IntroRoll_ctor(ILContext il)
        {
            var cursor = new ILCursor(il);

            try
            {
                if (cursor.TryGotoNext(i => i.MatchNewarr<string>()))
                {
                    // Declare the local index within the scope
                    int localIndex = -1;

                    if (cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(out localIndex)))
                    {
                        cursor.Emit(OpCodes.Ldloc, localIndex); // Load the local variable
                        cursor.EmitDelegate<Func<string[], string[]>>((oldTitleImages) =>
                        {
                            return oldTitleImages.Concat(new[]
                            {
                                "FFwhite", "FFred", "FFyellow", "Death", "DeathKarma",
                                "UnusedSpear", "Lights", "Dusk", "Night", "gormMurder",
                                "Sky", "ScavMarks"
                            }).ToArray();
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
