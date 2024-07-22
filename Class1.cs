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
        public void OnEnable()
        {
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        }
        public void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            customTitlesFilePath = AssetManager.ResolveFilePath("customTitles.txt");
            customTitlesDirectory = Path.GetDirectoryName(customTitlesFilePath) + "/illustrations";
            Logger.LogInfo($"Resolved custom titles file path: {customTitlesFilePath}");
            Logger.LogInfo($"Custom titles directory: {customTitlesDirectory}");
            IL.Menu.IntroRoll.ctor += IntroRoll_ctor;
        }
            
        public void IntroRoll_ctor(ILContext il)
        {
            var cursor = new ILCursor(il);

            if (cursor.TryGotoNext(i => i.MatchNewarr<string>()))
               
            {
                if(cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(3)))
                {
                    cursor.Emit(OpCodes.Ldloc_3);
                    cursor.EmitDelegate<Func<string[], string[]>>((oldTitleImages) => [.. oldTitleImages, "FFwhite", "FFred", "FFyellow", "Death", "DeathKarma",
"UnusedSpear", "Lights", "Dusk", "Night", "gormMurder", "Sky", "ScavMarks"]);
                    cursor.Emit(OpCodes.Stloc_3);
                }
                else
                {
                    Logger.LogError("you already know(no stloc(3))");
                }
            }
            else
            {
                Logger.LogMessage("can't find newarr");
            }
        }
    }
}
