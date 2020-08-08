using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal class SpriteImageInfo
    {
        public bool Vertical { get; private set; }
        public int FramesCount { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public string SpriteName { get; private set; }
        public int SpriteWidth { get; private set; }
        public int SpriteHeight { get; private set; }
        public static async ValueTask<SpriteImageInfo> Load(bool vertical, int framesCount, string spriteName, string spriteFileName, IJSRuntime jsRuntime)
        {
            var spriteImage = new SpriteImageInfo
            {
                SpriteName = spriteName,
                Vertical = vertical,
                FramesCount = framesCount,
            };

            var wh = await jsRuntime.InvokeAsync<int[]>("loadImage", spriteFileName, spriteName);
            spriteImage.SpriteWidth = wh[0];
            spriteImage.SpriteHeight = wh[1];

            if (spriteImage.Vertical)
            {
                spriteImage.FrameWidth = spriteImage.SpriteWidth;
                spriteImage.FrameHeight = spriteImage.SpriteHeight / spriteImage.FramesCount;
            }
            else
            {
                spriteImage.FrameWidth = spriteImage.SpriteWidth / spriteImage.FramesCount;
                spriteImage.FrameHeight = spriteImage.SpriteHeight;
            }

            return spriteImage;
        }
    }

    internal class GameResources
    {
        internal static async ValueTask<Dictionary<string, SpriteImageInfo>> LoadImages(IJSRuntime jsRuntime)
        {
            var images = new Dictionary<string, SpriteImageInfo>
            {
                ["Logo"] = await SpriteImageInfo.Load(false, 1, "Logo", "/images/Logo.jpg", jsRuntime),
                ["SeaBack"] = await SpriteImageInfo.Load(false, 1, "SeaBack", "/images/sea_bk.jpg", jsRuntime),
                ["SeaFront"] = await SpriteImageInfo.Load(false, 1, "SeaFront", "/images/sea_top.png", jsRuntime),
                ["Corrals"] = await SpriteImageInfo.Load(false, 1, "Corrals", "/images/corrals.png", jsRuntime),

                ["PlateLost"] = await SpriteImageInfo.Load(false, 1, "PlateLost", "/images/lost.png", jsRuntime),
                ["PlateWon"] = await SpriteImageInfo.Load(false, 1, "PlateWon", "/images/won.png", jsRuntime),
                ["PlatePause"] = await SpriteImageInfo.Load(false, 1, "PlatePause", "/images/pause.png", jsRuntime),

                ["WavesBack"] = await SpriteImageInfo.Load(false, 7, "WavesBack", "/images/wave_bk.png", jsRuntime),
                ["WavesFront"] = await SpriteImageInfo.Load(false, 7, "WavesFront", "/images/wave_top.png", jsRuntime),

                ["Trash1"] = await SpriteImageInfo.Load(false, 1, "Trash1", "/images/trash_1.png", jsRuntime),
                ["Trash2"] = await SpriteImageInfo.Load(false, 1, "Trash2", "/images/trash_2.png", jsRuntime),
                ["Trash3"] = await SpriteImageInfo.Load(false, 1, "Trash3", "/images/trash_3.png", jsRuntime),
                ["Trash4"] = await SpriteImageInfo.Load(false, 1, "Trash4", "/images/trash_4.png", jsRuntime),
                ["Trash5"] = await SpriteImageInfo.Load(false, 1, "Trash5", "/images/trash_5.png", jsRuntime),

                ["Fish1L"] = await SpriteImageInfo.Load(false, 7, "Fish1L", "/images/fish_1_left.png", jsRuntime),
                ["Fish1R"] = await SpriteImageInfo.Load(false, 7, "Fish1R", "/images/fish_1_right.png", jsRuntime),
                ["Fish2L"] = await SpriteImageInfo.Load(false, 7, "Fish2L", "/images/fish_2_left.png", jsRuntime),
                ["Fish2R"] = await SpriteImageInfo.Load(false, 7, "Fish2R", "/images/fish_2_right.png", jsRuntime),
                ["Fish3L"] = await SpriteImageInfo.Load(false, 7, "Fish3L", "/images/fish_3_left.png", jsRuntime),
                ["Fish3R"] = await SpriteImageInfo.Load(false, 7, "Fish3R", "/images/fish_3_right.png", jsRuntime),

                ["Bubble"] = await SpriteImageInfo.Load(true, 4, "Bubble", "/images/bubble.png", jsRuntime),

                ["DolphinFlowL"] = await SpriteImageInfo.Load(false, 5, "DolphinFlowL", "/images/dolphin_flow_left.png", jsRuntime),
                ["DolphinFlowR"] = await SpriteImageInfo.Load(false, 5, "DolphinFlowR", "/images/dolphin_flow_right.png", jsRuntime),
                ["DolphinEatL"] = await SpriteImageInfo.Load(false, 4, "DolphinEatL", "/images/dolphin_eat_left.png", jsRuntime),
                ["DolphinEatR"] = await SpriteImageInfo.Load(false, 4, "DolphinEatR", "/images/dolphin_eat_right.png", jsRuntime),
                ["DolphinDieL"] = await SpriteImageInfo.Load(true, 9, "DolphinDieL", "/images/dolphin_die_left.png", jsRuntime),
                ["DolphinDieR"] = await SpriteImageInfo.Load(true, 9, "DolphinDieR", "/images/dolphin_die_right.png", jsRuntime),

                ["Ship"] = await SpriteImageInfo.Load(false, 1, "Ship", "/images/ship.png", jsRuntime),
                ["ScrewS"] = await SpriteImageInfo.Load(false, 1, "ScrewS", "/images/screw_s.png", jsRuntime),
                ["ScrewL"] = await SpriteImageInfo.Load(false, 3, "ScrewL", "/images/screw_l.png", jsRuntime),
                ["ScrewR"] = await SpriteImageInfo.Load(false, 3, "ScrewR", "/images/screw_r.png", jsRuntime), 
                ["Arrow"] = await SpriteImageInfo.Load(false, 10, "Arrow", "/images/arrow_A.png", jsRuntime),
                ["Hug"] = await SpriteImageInfo.Load(false, 4, "Hug", "/images/hug_A.png", jsRuntime)
            };

            return images;
        }
    }
}
