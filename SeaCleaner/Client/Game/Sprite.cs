using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal class Sprite
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double DisplayWidth { get; private set; }
        public double DisplayHeight { get; private set; }
        public SpriteImageInfo Image { get; private set; }
        public Sprite(double x, double y, double w, double h, SpriteImageInfo image)
        {
            PosX = x;
            PosY = y;
            DisplayWidth = w;
            DisplayHeight = h;
            Image = image;
        }

        public virtual async ValueTask Draw(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync("drawImage", Image.SpriteName, PosX, PosY, DisplayWidth, DisplayHeight);
        }
    }

    internal class Animation
    {
        public double PosX { get; protected set; }
        public double PosY { get; protected set; }
        public SpriteImageInfo Image { get; private set; }
        public int UpdatesOnFrame { get; private set; }
        public double UpdatesShiftX { get; protected set; } 
        public double UpdatesShiftY { get; protected set; }
        private int _updatesCounter = 0;
        private int _currentFrame = 0;
        protected Random Rand { get; private set; } = new Random();

        public Animation(double x, double y, int updates, double shiftX, double shiftY, SpriteImageInfo image)
        {
            PosX = x;
            PosY = y;
            UpdatesOnFrame = updates;
            UpdatesShiftX = shiftX;
            UpdatesShiftY = shiftY;
            Image = image;
        }

        public virtual void Update()
        {
            PosX += UpdatesShiftX;
            PosY += UpdatesShiftY;

            _updatesCounter++;
            if (_updatesCounter >= UpdatesOnFrame)
            {
                if (Image.FramesCount <= _currentFrame + 1)
                    _currentFrame = 0;
                else
                    _currentFrame++;

                _updatesCounter = 0;
            }
        }

        private int framePosX;
        private int framePosY;

        public virtual async ValueTask Draw(IJSRuntime jsRuntime)
        {
            framePosX = Image.Vertical ? 0 : _currentFrame * Image.FrameWidth;
            framePosY = Image.Vertical ? _currentFrame * Image.FrameHeight : 0;

            await jsRuntime.InvokeVoidAsync("drawSprite", Image.SpriteName, framePosX, framePosY, Image.FrameWidth, Image.FrameHeight, PosX, PosY);
        }
    }

    internal class Bubble : Animation
    {
        public double Fluctuation { get; private set; }  
        private readonly double _fluctCoeff = 0.1;
        public Bubble(double x, double y, int updates, double shiftY, double fluctuation, SpriteImageInfo imageInfo) : base(x, y, updates, 0, shiftY, imageInfo)
        {
            Fluctuation = fluctuation;
        }

        public override void Update()
        {
            PosX += Math.Sin(PosY * _fluctCoeff) * Fluctuation;

            if (PosY <= 150)
            {
                PosY = 720 + Math.Round(Rand.NextDouble() * 1000) % 30;
                PosX = Rand.NextDouble() * Game.SCREEN_WIDTH;
            }
            base.Update();
        }
    }

    internal class Fishes : Animation
    {
        public bool ToLeft { get; private set; }
        public Fishes(int updates, double shiftX, bool toLeft, SpriteImageInfo imageInfo): base (300, 200, updates, shiftX, 0, imageInfo)
        {
            ToLeft = toLeft;            
        }

        public void Initialize()
        {            
            UpdatesShiftX = Math.Round(Rand.NextDouble() * 1000) % 4 + 3;
            PosY = Rand.NextDouble() * 1000 % 480 + 170;

            if (ToLeft)
            {
                PosX = 1050 + Math.Round(Rand.NextDouble() * 1000 / 3);
                if (UpdatesShiftX > 0) UpdatesShiftX = -UpdatesShiftX;
            }
            else
            {
                PosX = -50 - Math.Round(Rand.NextDouble() * 1000 / 3);
                if (UpdatesShiftX < 0) UpdatesShiftX = -UpdatesShiftX;
            }
        }

        public bool IsOffScreen() => (ToLeft, PosX < -50, PosX > 1050) switch
        {
            (true, true, _) => true,
            (true, false, _) => false,
            (false, _, true) => true,
            (false, _, false) => false
        };
    }
}
