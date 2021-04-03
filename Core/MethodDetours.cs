﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerrariaAmbience.Content;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Audio;
using Terraria.ID;
using Terraria.UI.Chat;
using TerrariaAmbience.Content.Players;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace TerrariaAmbience.Core
{
    internal class MethodDetours
    {
        public static void DetourAll()
        {
            On.Terraria.Main.DrawMenu += Main_DrawMenu;
            On.Terraria.Main.DrawInterface_30_Hotbar += Main_DrawInterface_30_Hotbar;
            On.Terraria.IngameOptions.Draw += IngameOptions_Draw;
            On.Terraria.IngameOptions.DrawRightSide += IngameOptions_DrawRightSide;
            On.Terraria.IngameOptions.DrawLeftSide += IngameOptions_DrawLeftSide;

            active = true;
            posY = 4;
        }
        private static bool IngameOptions_DrawLeftSide(On.Terraria.IngameOptions.orig_DrawLeftSide orig, SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float[] scales, float minscale, float maxscale, float scalespeed)
        {
            if (i == 0)
            {
                sb.DrawString(Main.fontMouseText, $"Hold Add or Subtract to change the volume of Terraria Ambience!", new Vector2(8, 8), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1f);
            }
            return orig(sb, txt, i, anchor, offset, scales, minscale, maxscale, scalespeed);
        }

        private static bool oldHover;
        private static bool hovering;
        private static bool IngameOptions_DrawRightSide(On.Terraria.IngameOptions.orig_DrawRightSide orig, SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float scale, float colorScale, Color over)
        {
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Add))
            {
                Ambience.TAAmbient += 0.025f;
            }
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Subtract))
            {
                Ambience.TAAmbient -= 0.025f;
            }

            Rectangle hoverPos = new Rectangle((int)anchor.X - 65, (int)anchor.Y + 119, 275, 15);
            if (i == 14)
            {
                hovering = hoverPos.Contains(Main.MouseScreen.ToPoint());
            }

            var svol = (float)System.Math.Round(Ambience.TAAmbient);
            svol = MathHelper.Clamp(svol, 0f, 100f);
            Ambience.TAAmbient = MathHelper.Clamp(Ambience.TAAmbient, 0f, 100f);
            string percent = $"TA Ambient: {svol}%";
            if (!oldHover && hovering)
            {
                Main.PlaySound(SoundID.MenuTick);
            }
            var center = (Main.fontMouseText.MeasureString(percent) * Main.UIScale) / 2; 
            if (i == 14 && IngameOptions.category == 0)
            {
                orig(sb, percent, i, anchor, offset, scale, colorScale, over);
            }

            oldHover = hovering;

            if (IngameOptions.category == 2)
            {
                if (i == 3)
                {
                    if (Main.FrameSkipMode == 2)
                    {
                        txt = "Frame Skip Subtle (WARNING)";
                    }
                }
            }
            return orig(sb, txt, i, anchor, offset, scale, colorScale, over);
        }

        private static void IngameOptions_Draw(On.Terraria.IngameOptions.orig_Draw orig, Main mainInstance, SpriteBatch sb)
        {
            orig(mainInstance, sb);
        }

        private static Vector2 drawPos;
        private static string displayable;
        private static void Main_DrawInterface_30_Hotbar(On.Terraria.Main.orig_DrawInterface_30_Hotbar orig, Main self)
        {
            orig(self);

            // Main.spriteBatch.Begin();
            var loader = Ambience.Instance;

            var aPlayer = Main.player[Main.myPlayer].GetModPlayer<FootstepsPlayer>();
            if (aPlayer.soundInstanceSnowStep != null && aPlayer.soundInstanceWoodStep != null && aPlayer.soundInstanceStoneStep != null && aPlayer.soundInstanceGrassStep != null && aPlayer.soundInstanceSandStep != null)
            {
                displayable = ModAmbience.modAmbienceList.Count <= 0 && ModAmbience.allAmbiences.Count <= 0 ? 
                    $"{loader.BeachWaves.Name}: {loader.BeachWavesInstance.Volume}"
                    + $"\n{loader.CampfireCrackle.Name}: {loader.crackleVolume}"
                    + $"\n{loader.SnowBreezeDay.Name}: {loader.SnowBreezeDayInstance.Volume}"
                    + $"\n{loader.SnowBreezeNight.Name}: {loader.SnowBreezeNightInstance.Volume}"
                    + $"\n{loader.MorningCrickets.Name}: {loader.MorningCricketsInstance.Volume}"
                    + $"\n{loader.DayCrickets.Name}: {loader.DayCricketsInstance.Volume}"
                    + $"\n{loader.NightCrickets.Name}: {loader.NightCricketsInstance.Volume}"
                    + $"\n{loader.EveningCrickets.Name}: {loader.EveningCricketsInstance.Volume}"
                    + $"\n{loader.CavesAmbience.Name}: {loader.CavesAmbienceInstance.Volume}"
                    + $"\n{loader.CrimsonRumbles.Name}: {loader.CrimsonRumblesInstance.Volume}"
                    + $"\n{loader.CorruptionRoars.Name}: {loader.CorruptionRoarsInstance.Volume}"
                    + $"\n{loader.DayJungle.Name}: {loader.DaytimeJungleInstance.Volume}"
                    + $"\n{loader.NightJungle.Name}: {loader.NightJungleInstance.Volume}"
                    + $"\n{loader.DesertAmbience.Name}: {loader.DesertAmbienceInstance.Volume}"
                    + $"\n{loader.HellRumble.Name}: {loader.HellRumbleInstance.Volume}"
                    + $"\n{loader.Rain.Name}: {loader.RainInstance.Volume}"
                    + $"\n{loader.Breeze.Name}: {loader.BreezeInstance.Volume}"
                    + $"\nFootsteps:\nGrass: {aPlayer.soundInstanceGrassStep.State}"
                    + $"\nStone: {aPlayer.soundInstanceStoneStep.State}"
                    + $"\nWood: {aPlayer.soundInstanceWoodStep.State}"
                    + $"\nSnow: {aPlayer.soundInstanceSnowStep.State}"
                    + $"\nSand: {aPlayer.soundInstanceSandStep.State}"
                    :
               displayable = $"{loader.BeachWaves.Name}: {loader.BeachWavesInstance.Volume}"
                    + $"\n{loader.CampfireCrackle.Name}: {loader.crackleVolume}"
                    + $"\n{loader.SnowBreezeDay.Name}: {loader.SnowBreezeDayInstance.Volume}"
                    + $"\n{loader.SnowBreezeNight.Name}: {loader.SnowBreezeNightInstance.Volume}"
                    + $"\n{loader.MorningCrickets.Name}: {loader.MorningCricketsInstance.Volume}"
                    + $"\n{loader.DayCrickets.Name}: {loader.DayCricketsInstance.Volume}"
                    + $"\n{loader.NightCrickets.Name}: {loader.NightCricketsInstance.Volume}"
                    + $"\n{loader.EveningCrickets.Name}: {loader.EveningCricketsInstance.Volume}"
                    + $"\n{loader.CavesAmbience.Name}: {loader.CavesAmbienceInstance.Volume}"
                    + $"\n{loader.CrimsonRumbles.Name}: {loader.CrimsonRumblesInstance.Volume}"
                    + $"\n{loader.CorruptionRoars.Name}: {loader.CorruptionRoarsInstance.Volume}"
                    + $"\n{loader.DayJungle.Name}: {loader.DaytimeJungleInstance}"
                    + $"\n{loader.NightJungle.Name}: {loader.NightJungleInstance.Volume}"
                    + $"\n{loader.DesertAmbience.Name}: {loader.DesertAmbienceInstance.Volume}"
                    + $"\n{loader.HellRumble.Name}: {loader.HellRumbleInstance.Volume}"
                    + $"\n{loader.Rain.Name}: {loader.RainInstance.Volume}"
                    + $"\n{loader.Breeze.Name}: {loader.BreezeInstance.Volume}"
                    + $"\nFootsteps:\nGrass: {aPlayer.soundInstanceGrassStep.State}"
                    + $"\nStone: {aPlayer.soundInstanceStoneStep.State}"
                    + $"\nWood: {aPlayer.soundInstanceWoodStep.State}"
                    + $"\nSnow: {aPlayer.soundInstanceSnowStep.State}"
                    + $"\nSand: {aPlayer.soundInstanceSandStep.State}\nModded Sounds:";


                int index = 0;
                foreach (ModAmbience ambient in ModAmbience.modAmbienceList)
                {
                    if (ambient != null)
                    {
                        if (ModAmbience.modAmbienceList.Count > 0)
                        {
                            index = ModAmbience.allAmbiences.Count;
                            displayable += $"\n{ambient.Name}: {ambient.Volume}";
                        }
                    }
                }
                foreach (ModAmbience ambient in ModAmbience.allAmbiences)
                {
                    if (ambient != null)
                    {
                        if (ModAmbience.allAmbiences.Count > 0)
                        {
                            index = ModAmbience.allAmbiences.Count;
                            displayable = string.Concat(displayable, $"\n{ambient.Name}: {ambient.Volume}");
                        }
                    }
                }
            }

            if (ModContent.GetInstance<AmbientConfigClient>().volVals)
            {
                if (Main.playerInventory && (Main.mapStyle == 0 || Main.mapStyle == 2))
                {
                    drawPos = new Vector2(Main.screenWidth - Main.screenHeight / 2, 175);
                }
                if (Main.mapStyle == 1 && Main.playerInventory)
                {
                    drawPos = new Vector2(Main.screenWidth - Main.screenHeight / 2, 375);
                }
                if (Main.mapStyle == 1 && !Main.playerInventory)
                {
                    drawPos = new Vector2(Main.screenWidth - Main.screenHeight / 3, 375);
                }
                if ((Main.mapStyle == 0 || Main.mapStyle == 2) && !Main.playerInventory)
                {
                    drawPos = new Vector2(Main.screenWidth - Main.screenHeight / 3, 175);
                }

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, 
                    Main.fontDeathText, 
                    displayable != default ? displayable : "Sounds not valid", 
                    position: drawPos, 
                    Color.LightGray, 
                    0f, 
                    origin: Vector2.Zero, 
                    baseScale: new Vector2(0.25f, 0.25f), 
                    -1, 
                    1);
            }
        }
        private static float posX;
        private static float posY;
        private static bool active;
        private static void Main_DrawMenu(On.Terraria.Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            Texture2D arrow = ModContent.GetInstance<Content.TerrariaAmbience>().GetTexture("Content/UI/UIButtonRight");

            // Echcode.tm
            /*int i = 0;
            int j = 1;
            int z = 0;
            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod != null && mod.TextureExists("icon"))
                {
                    i++;
                    if (((100 * i) + 95) < Main.screenHeight)
                    {
                        Main.spriteBatch.SafeDraw(mod.GetTexture("icon"), new Vector2(80, 100 * i), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, mod.DisplayName, new Vector2(120, (100 * i) + 95), Color.LightGray, 0f, Main.fontDeathText.MeasureString(mod.DisplayName) / 2, new Vector2(0.35f, 0.35f), 0, 1);
                    }
                    else
                    {
                        j++;
                        z++;
                        Main.spriteBatch.Draw(mod.GetTexture("icon"), new Vector2(100 * j, (100 * i) - (z * 900)), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, mod.DisplayName, new Vector2(120 * j, ((100 * i) + 95) - (z * 900)), Color.LightGray, 0f, Main.fontDeathText.MeasureString(mod.DisplayName) / 2, new Vector2(0.35f, 0.35f), 0, 1);
                    }
                }
            }*/

            posX = MathHelper.Clamp(posX, -450, -16);
            var sb = Main.spriteBatch;
            string viewPost = "View the Terraria Ambience forums post here:";
            string forums = $"Forums Post";

            var click2Activate = new Rectangle((int)posX + 455, (int)posY, 12, 20);

            if (Main.menuMode == 0) Main.spriteBatch.SafeDraw(arrow, new Vector2(posX + 455, posY), null, Color.White, 0f, Vector2.Zero, 0.6f, !active ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1f);
            var rect = new Rectangle((int)posX + 350, (int)posY, (int)(Main.fontDeathText.MeasureString(forums).X * 0.35f), (int)(Main.fontDeathText.MeasureString(forums).Y * 0.25f));
            // Main.spriteBatch.Draw(Main.magicPixel, click2Activate, Color.White * 0.35f);
            bool hovering = rect.Contains(Main.MouseScreen.ToPoint());
            bool hoverAct = click2Activate.Contains(Main.MouseScreen.ToPoint());

            if (Main.menuMode == 0)
            {
                if (hoverAct)
                {
                    if (Main.mouseRight)
                    {
                        if (Main.MouseScreen.Y < Main.screenHeight && Main.MouseScreen.Y > 0)
                        {
                            posY = Main.MouseScreen.Y - 10;
                        }
                    }
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        active = !active;
                    }
                }
            }
            posX += active ? 20f : -20f;

            if (Main.menuMode == 0) ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, viewPost, new Vector2(posX, posY), Color.LightGray, 0f, Vector2.Zero, new Vector2(0.35f, 0.35f), 0, 1);
            if (Main.menuMode == 0) ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, forums, new Vector2(posX + (int)(Main.fontDeathText.MeasureString(viewPost).X * 0.35f) + 10, posY), hovering ? Color.White : Color.Gray, 0f, Vector2.Zero, new Vector2(0.35f, 0.35f), 0, 1);

            var svol = (float)Math.Round(Ambience.TAAmbient);
            svol = MathHelper.Clamp(svol, 0f, 100f);
            Ambience.TAAmbient = MathHelper.Clamp(Ambience.TAAmbient, 0f, 100f);
            string percent = $"TA Ambient: {svol}%";

            var pos = new Vector2(Main.screenWidth / 2, 435);
            if (Main.menuMode == 26)
            {
                sb.DrawString(Main.fontMouseText, $"Hold Add or Subtract to change the volume of Terraria Ambience!", new Vector2(6, 22), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1f);

                ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, percent, pos, Color.LightGray, 0f, Main.fontDeathText.MeasureString(percent) / 2, new Vector2(0.6f, 0.6f), 0, 2);

                if (Main.keyState.IsKeyDown(Keys.Add))
                {
                    Ambience.TAAmbient += 0.5f;
                }
                if (Main.keyState.IsKeyDown(Keys.Subtract))
                {
                    Ambience.TAAmbient -= 0.5f;
                }
            }
            if (hovering)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (active)
                    {
                        System.Diagnostics.Process.Start("https://forums.terraria.org/index.php?threads/terraria-ambience-mod.104161/");
                    }
                }
            }
            orig(self, gameTime);
        }
    }
}
