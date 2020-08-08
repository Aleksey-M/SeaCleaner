using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal class Waves
    {
        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public double WavesShift { get; private set; }
        public int WavesWidth { get; private set; }
        public int UpdatesCountForFrame { get; private set; }
        public SpriteImageInfo FrontImage { get; private set; }
        public SpriteImageInfo BackImage { get; private set; }
        public int FramesCountInRow => WavesWidth / BackImage.FrameWidth + 1;
        private int _currentFrame;
        private int _updatesCounter;

        public Waves(double posY, double wavesShift, int wavesWidth, int updatesCountForFrameSwitching, SpriteImageInfo frontImage, SpriteImageInfo backImage)
        {
            PosY = posY;
            WavesShift = wavesShift;
            WavesWidth = wavesWidth;
            FrontImage = frontImage;
            BackImage = backImage;
            UpdatesCountForFrame = updatesCountForFrameSwitching;
        }

        public void Update()
        {
            if (++_updatesCounter >= UpdatesCountForFrame)
            {
                PosX += WavesShift;
                if (PosX >= 0)
                    PosX = -BackImage.FrameWidth + WavesShift;
                else
                {
                    _currentFrame++;
                    if (_currentFrame >= BackImage.FramesCount) _currentFrame = 0;
                }
                _updatesCounter = 0;
            }
        }

        private async ValueTask Draw(SpriteImageInfo image, IJSRuntime jsRuntime)
        {
            double x = PosX;
            double y = PosY;
            double picX = _currentFrame * image.FrameWidth;
            double picY = 0;
            for (var i = 0; i < FramesCountInRow; i++)
            {
                await jsRuntime.InvokeVoidAsync(
                    "drawSprite",
                    image.SpriteName,
                    picX,
                    picY,
                    image.FrameWidth,
                    image.FrameHeight,
                    x, y);

                x += image.FrameWidth;
                picX += image.FrameWidth;
                if (picX >= image.FramesCount * image.FrameWidth) picX = 0;
            }
        }

        public ValueTask DrawBack(IJSRuntime jsRuntime) => Draw(BackImage, jsRuntime);
        public ValueTask DrawFront(IJSRuntime jsRuntime) => Draw(FrontImage, jsRuntime);
    }
}
