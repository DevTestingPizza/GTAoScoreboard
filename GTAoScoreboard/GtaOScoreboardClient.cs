using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace GTAoScoreboard
{
    public class GtaOScoreboardClient : BaseScript
    {

        /* I originally wanted to get the wanted level stars to show up, but they looked crap so I decided to remove them.
        /* You can use these variables to implement this feature yourself if you want to.
         * 
        /*  private const string STAR_TEXTURE_DICT = "mpleaderboard";
        /*  private const string STAR_TEXTURE_NAME = "leaderboard_star_icon";
         *  private bool firstTick = true;
        /*/

        // Global variables
        private enum DisplayType { LEFT, CENTER, RIGHT };
        private enum CurrentPage { HIDDEN, FIRST, SECOND };
        private DisplayType displayType = DisplayType.LEFT;
        private CurrentPage currPage = CurrentPage.HIDDEN;
        private int timer = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public GtaOScoreboardClient()
        {
            GetConfig();
            Tick += OnTick;
        }

        private async Task GetConfig()
        {
            await Delay(500);
            string config = GetResourceMetadata(GetCurrentResourceName(), "displayType", 0);
            switch (config.ToLower())
            {
                case "center":
                    displayType = DisplayType.CENTER;
                    break;
                case "right":
                    displayType = DisplayType.RIGHT;
                    break;
            }
        }

        /// <summary>
        /// OnTick.
        /// Runs every game tick.
        /// </summary>
        /// <returns></returns>
        public async Task OnTick()
        {
            /* UNUSED, CAN BE USED IF YOU WANT TO IMPLEMENT WANTED LEVELS TO THE SCOREBOARD.
            // If the script is ran for the first time.
            if (firstTick)
            {
                firstTick = false;
                // Load the texture dictionary for the wanted level star.
                RequestStreamedTextureDict(STAR_TEXTURE_DICT, false);
                // Wait until the loading done.
                while (!HasStreamedTextureDictLoaded(STAR_TEXTURE_DICT))
                {
                    await Delay(0);
                }
            } */


            int numPlayers = NetworkGetNumConnectedPlayers();

            // If Z on keyboard or dpad-down on controller is pressed, toggle scoreboard.
            if (IsControlJustPressed(0, (int)Control.MultiplayerInfo))
            {
                // If the scoreboard is hidden, then show the 1st page.
                if (currPage == CurrentPage.HIDDEN)
                {
                    currPage = CurrentPage.FIRST;
                }
                // If the 1st page is showing and there is more than 16 players online, show the second page.
                else if (currPage == CurrentPage.FIRST && numPlayers > 16)
                {
                    //else if(currPage == CurrentPage.FIRST && NetworkGetNumConnectedPlayers() > 16){
                    currPage = CurrentPage.SECOND;
                }
                // Otherwise, show nothing.
                else
                {
                    currPage = CurrentPage.HIDDEN;

                    // Set a 250 ms delay to prevent accidentally reopening instantly after closing it.
                    await Delay(250);
                }
                // Anytime the scoreboard is toggled, reset the scoreboard-open-timer.
                timer = 0;
            }

            // If the scoredboard page is 1st or 2nd page, the timer is below 400 and the pause menu is not active,
            // then show a page and increment timer by 1.
            if (currPage != CurrentPage.HIDDEN && timer < 400 && !IsPauseMenuActive())
            {
                // Increment the timer.
                timer++;
                // Show the scoreboard's current page.
                ShowScoreboard(currPage);
            }
            else
            {
                timer = 0;
                currPage = CurrentPage.HIDDEN;
            }
        }

        /// <summary>
        /// Show the page provided by p1.
        /// </summary>
        /// <param name="page"></param>
        private void ShowScoreboard(CurrentPage page)
        {
            // If this gets called, then at least one scoreboard should be visible,
            // so always draw the header.
            string headerText = "~|~Players Online ( " + NetworkGetNumConnectedPlayers() + " )";
            string headerSecondaryText = "Server ID";
            DrawRow(headerText, headerSecondaryText, size: 0.35f, font: 0);

            // Loop through all players.
            var scoreboardIndex = 0;
            for (var i = 0; i < 32; i++)
            {
                // If the player exists...
                if (NetworkIsPlayerActive(i))
                {
                    // Get the player's info.
                    var name = GetPlayerName(i);
                    var serverId = GetPlayerServerId(i);

                    // Again, this is unused and needs some work before it is actually useful.
                    //var wantedLevel = GetPlayerWantedLevel(i);
                    var wantedLevel = 0;

                    // If the first page should be displayed, then do this:
                    if (scoreboardIndex < 16 && page == CurrentPage.FIRST)
                    {
                        if (i == PlayerId())
                        {
                            DrawRow(name, serverId.ToString(), 0.04f * (scoreboardIndex + 1), r: 75, g: 150, b: 225, stars: wantedLevel, bgColor: (serverId % 2 == 0) ? 15 : 20);
                        }
                        else
                        {
                            DrawRow(name, serverId.ToString(), 0.04f * (scoreboardIndex + 1), stars: wantedLevel, bgColor: (serverId % 2 == 0) ? 15 : 20);
                        }
                    }
                    // If the second page should be displayed, then do this:
                    else if (scoreboardIndex > 15 && page == CurrentPage.SECOND)
                    {
                        DrawRow(name, serverId.ToString(), 0.04f * (scoreboardIndex - 15), stars: wantedLevel, bgColor: (serverId % 2 == 0) ? 15 : 20);
                    }

                    // Player is valid, so increment scoreboardIndex by 1
                    scoreboardIndex++;
                }
            }
        }

        /// <summary>
        /// Draw Row.
        /// Used both for drawing the header as well as the other rows.
        /// </summary>
        /// <param name="leftText">Text that displays on the left side of the row.</param>
        /// <param name="rightText">Text that displays on the right side of the row.</param>
        /// <param name="starty">The y position.</param>
        /// <param name="r">The text color (red rgb value)</param>
        /// <param name="g">The text color (green rgb value)</param>
        /// <param name="b">The text color (blue rgb value)</param>
        /// <param name="stars">The wantedlevel of the player. (unused for now)</param>
        /// <param name="bgColor">The background shade (value used for red, green and blue).</param>
        /// <param name="size">The text font size.</param>
        /// <param name="font">The text font id.</param>
        private void DrawRow(string leftText, string rightText, float starty = 0.0f, int r = 255, int g = 255, int b = 255, int stars = 0, int bgColor = 2, float size = 0.45f, int font = 6)
        {
            // Constants.
            const int alpha = 200;

            // Variables.
            float width = 0.2f;
            float height = 0.04f;
            float safeZoneOffset = (GetSafeZoneSize() / 2.5f) - 0.4f;
            float y = starty + (height / 2) - safeZoneOffset;
            int red = bgColor;
            int green = bgColor;
            int blue = bgColor;
            float x = (width / 2) - safeZoneOffset;

            // Display header on the left.
            if (displayType == DisplayType.LEFT)
            {
                x = (width / 2) - safeZoneOffset;
            }
            // Display header in the middle.
            else if (displayType == DisplayType.CENTER)
            {
                x = (width / 2) + 0.4f;
            }
            // Display header on the right.
            else if (displayType == DisplayType.RIGHT)
            {
                x = 1.0f - (width / 2) + safeZoneOffset;
            }
            DrawRect(x, y, width, height, red, green, blue, alpha);

            DrawText(leftText, x - (width / 2) + 0.005f, y - (height / 2) + 0.005f, x + width, 1, r, g, b, size: size, font: font);
            DrawText(rightText, x - (width / 2), y - (height / 2) + 0.005f, x + (width / 2) - 0.005f, 2, r, g, b, size: size, font: font);
        }

        /// <summary>
        /// Used to draw the text for each row in the scoreboard.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rightX"></param>
        /// <param name="justification"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="size"></param>
        /// <param name="font"></param>
        private void DrawText(string text, float x, float y, float rightX, int justification = 1, int r = 255, int g = 255, int b = 255, float size = 0.45f, int font = 6)
        {
            SetTextWrap(x, rightX);
            SetTextFont(font);
            SetTextScale(1.0f, size);
            SetTextJustification(justification);
            SetTextColour(r, g, b, 255);
            BeginTextCommandDisplayText("STRING");
            AddTextComponentSubstringPlayerName(text);
            EndTextCommandDisplayText(x, y);
        }

    }
}
