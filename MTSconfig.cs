using Menu;
using Menu.Remix.MixedUI;
using System.IO;
using System.Linq;
using UnityEngine;

namespace More_title_screens
{

    public class MTSconfig : OptionInterface
    {
        public static MTSconfig Instance { get; } = new();
        public static Configurable<bool> OnlyCustomTitles;
        MTSconfig()
        {
            OnlyCustomTitles = config.Bind("OnlyCustomTitles", true, new ConfigurableInfo( "The game will only show custom titles screens (no downpour screens)", tags: [
               
               "Only custom title screens"
           ]));
        }
        
        // Called when the config menu is opened by the player.
        public override void Initialize()
        {
            base.Initialize();
            Tabs =
            [
                new OpTab(this, "Options"),
            ];
            AddDivider(593f);
            AddTitle(0);
            AddDivider(557f);
            //AddCheckbox(OnlyCustomTitles, 440f);
        }
        private void AddTitle(int tab, string text = "More Title Screens", float yPos = 560f)
        {
            OpLabel title = new(new Vector2(150f, yPos), new Vector2(300f, 30f), text, bigText: true);

            Tabs[tab].AddItems(
            [
                title
            ]);
        }
        private void AddCheckbox(Configurable<bool> optionText, float y)
        {
            OpCheckBox checkbox = new(optionText, new Vector2(150f, y))
            {
                description = optionText.info.description
            };

            OpLabel checkboxLabel = new(150f + 40f, y + 2f, optionText.info.Tags[0] as string)
            {
                description = optionText.info.description
            };

            Tabs[0].AddItems(
            
                checkbox,
                checkboxLabel
            );
        }
        private void AddDivider(float y, int tab = 0)
        {
            OpImage dividerLeft = new(new Vector2(300f, y), "LinearGradient200");
            dividerLeft.sprite.SetAnchor(0.5f, 0f);
            dividerLeft.sprite.rotation = 270f;

            OpImage dividerRight = new(new Vector2(300f, y), "LinearGradient200");
            dividerRight.sprite.SetAnchor(0.5f, 0f);
            dividerRight.sprite.rotation = 90f;

            Tabs[tab].AddItems(
            [
                dividerLeft,
                dividerRight
            ]);
        }
    }
}
