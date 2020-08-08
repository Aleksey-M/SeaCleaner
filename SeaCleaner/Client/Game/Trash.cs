using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal class BoundingBox
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BoundingBox(double x, double y, int width, int height)
        {
            PosX = x;
            PosY = y;
            Width = width;
            Height = height;
        }

        public bool IsPointInside(double pointX, double pointY) =>
           (pointX >= PosX) && (pointX <= PosX + Width) && (pointY >= PosY) && (pointY <= PosY + Height);

        public bool IsIntersect(BoundingBox other) =>
            IsPointInside(other.PosX, other.PosY) ||
            IsPointInside(other.PosX + other.Width, other.PosY) ||
            IsPointInside(other.PosX, other.PosY + other.Height) ||
            IsPointInside(other.PosX + other.Width, other.PosY + other.Height);
    }

    internal class Trash
    {
        private readonly SpriteImageInfo _imageInfo;
        public BoundingBox BBox { get; private set; }
        public Trash(SpriteImageInfo imageInfo)
        {
            _imageInfo = imageInfo;
            _random = new Random();
            _posX = Math.Round(_random.NextDouble() * 1000) % 800 + 100;
            _posYWeggle = Math.Round(_random.NextDouble() * 1000) % 450 + 200;
            _posYStatic = _posYWeggle;
            _fCounter = Math.Round(_random.NextDouble() * 10000) % 1800;
            BBox = new BoundingBox(_posX, _posYWeggle, _imageInfo.SpriteWidth, _imageInfo.SpriteHeight);
        }

        private readonly Random _random;
        private double _posX;
        private double _posYWeggle;
        private double _posYStatic;
        private readonly double _fluctCoef = 0.1;
        private readonly double _fluctuation = 0.5;
        private double _fCounter;
        private int _counter = 0;

        public bool IsCaptured { get; set; }
        public bool IsVisible { get; set; } = true;
    
        public double PosX
        {
            get => _posX;
            set
            {
                _posX = value;
                BBox.PosX = value;
            }
        }

        public double PosY
        {
            get => _posYWeggle;
            set
            {
                _posYWeggle = value;
                _posYStatic = value;
                BBox.PosY = value;
            }
        } 

        public void Update()
        {
            if (IsCaptured) return;

            if (++_counter >= 2)
            {
                _fCounter++;
                if (_fCounter >= 1800)
                {
                    _fCounter = 0;
                    _posYWeggle = _posYStatic; // для корректировки положения, которое отклоняется из-за округлений при расчетах покачивания
                }
                _counter = 0;
            }

            if (!IsCaptured)
            {
                // покачивание вверх-вниз
                _posYWeggle += Math.Sin(_fCounter * _fluctCoef) * _fluctuation;
                BBox.PosY = _posYWeggle;
            }
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            if (IsVisible)
            {
                await jsRuntime.InvokeVoidAsync("drawImage", _imageInfo.SpriteName, PosX, PosY, _imageInfo.SpriteWidth, _imageInfo.SpriteHeight);
                if (Game.SHOW_BB)
                {
                    await jsRuntime.InvokeVoidAsync("drawRect", BBox.PosX, BBox.PosY, BBox.Width, BBox.Height);
                }
            }
        }
    }
}
