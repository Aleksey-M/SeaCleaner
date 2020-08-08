using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal enum DolphinState { Flow, Eat, Die, Drops, Out }

    internal class Dolphin
    {
        const int DOLPHIN_FLOW_UPDATES = 6;
        const int DOLPHIN_EAT_UPDATES = 5;
        const int DOLPHIN_DIE_UPDATES = 7;

        public static double _dPos = 0;

        private readonly SpriteImageInfo _imgDolphinFlow;
        private readonly SpriteImageInfo _imgDolphinEat;
        private readonly SpriteImageInfo _imgDolphinDie;
        private readonly bool _toLeft;
        private readonly Random _random;

        private double _shiftX = 2;
        private double _shiftY = 2;
        private int _counter = 0;
        private int _currentFrame = 0;

        public Trash CapturedTrash { get; set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }

        public BoundingBox BodyBB { get; private set; }
        public BoundingBox MouthBB { get; private set; }

        private Action _checkLost = () => { };
        private Action _checkWon = () => { };

        private DolphinState _state = DolphinState.Flow;
        public DolphinState State
        {
            get => _state;
            set
            {
                _counter = 0;
                switch (value)
                {
                    case DolphinState.Eat:
                        _currentFrame = _toLeft ? 6 : 0;
                        break;

                    case DolphinState.Die:
                        _currentFrame = 0;
                        if (CapturedTrash != null)
                        {
                            CapturedTrash.IsVisible = false;
                            CapturedTrash.IsCaptured = true;
                        }
                        break;

                    case DolphinState.Drops:
                        _currentFrame = _imgDolphinDie.FramesCount - 1;
                        break;

                    case DolphinState.Out:
                        _state = value;
                        _checkLost();
                        _checkWon();
                        break;
                }

                _state = value;
            }
        }

        public Dolphin(bool toLeft, SpriteImageInfo imgFlow, SpriteImageInfo imgEat, SpriteImageInfo imgDie, Action checkLost, Action checkWon)
        {
            _toLeft = toLeft;
            _imgDolphinFlow = imgFlow;
            _imgDolphinEat = imgEat;
            _imgDolphinDie = imgDie;
            _checkLost = checkLost;
            _checkWon = checkWon;
            _random = new Random();

            if (_toLeft) _shiftX = -_shiftX;
        }

        public void Initialize()
        {
            if (_toLeft)
            {
                PosX = 1000 + _dPos;
                _dPos += 500;
                PosY = Math.Round(_random.NextDouble() * 1000) % 480 + 200;

                BodyBB = new BoundingBox(PosX, PosY + 10, _imgDolphinFlow.FrameWidth - 20, _imgDolphinFlow.FrameHeight - 20);
                MouthBB = new BoundingBox(PosX, PosY + 25, 20, 25);

                _currentFrame = _imgDolphinFlow.FramesCount - 1;
            }
            else
            {
                PosX = -150 - _dPos;
                _dPos += 500;
                PosY = Math.Round(_random.NextDouble() * 1000) % 480 + 200;

                BodyBB = new BoundingBox(PosX, PosY + 10, _imgDolphinFlow.FrameWidth - 20, _imgDolphinFlow.FrameHeight - 20);
                MouthBB = new BoundingBox(PosX + 130, PosY + 25, 20, 25);
            }
        }

        public void UpdateBoundingBoxes()
        {
            MouthBB.PosY = PosY + 25;
            BodyBB.PosY = PosY + 10;

            MouthBB.PosX = _toLeft ? PosX : PosX + 130;
            BodyBB.PosX = _toLeft ? PosX : PosX + 20;
        }

        public void Update()
        {
            _counter++;
            switch (State)
            {
                case DolphinState.Flow:
                    if (_counter == DOLPHIN_FLOW_UPDATES)
                    {
                        _counter = 0;
                        if (!_toLeft)
                        {
                            if (++_currentFrame == _imgDolphinFlow.FramesCount) 
                                _currentFrame = 0;
                        }
                        else
                        {
                            if (--_currentFrame == -1) 
                                _currentFrame = _imgDolphinFlow.FramesCount - 1;
                        }                            
                    }

                    PosX += _shiftX;

                    if (_toLeft && PosX <= -150)
                    {
                        PosX = 1000 + 500;
                        PosY = Math.Round(_random.NextDouble() * 1000) % 480 + 200;
                    }

                    if (!_toLeft && PosX >= 1000)
                    {
                        PosX = -150 - 500;
                        PosY = Math.Round(_random.NextDouble() * 1000) % 480 + 200;
                    }

                    UpdateBoundingBoxes();
                    break;

                case DolphinState.Eat:
                    if (_counter == DOLPHIN_EAT_UPDATES)
                    {
                        _counter = 0;
                        if (_toLeft)
                        {
                            _currentFrame--;
                            if (_currentFrame == -1) 
                                State = DolphinState.Die;
                        }
                        else
                        {
                            _currentFrame++;
                            if (_currentFrame == 7)
                                State = DolphinState.Die;
                        }
                    }
                    break;

                case DolphinState.Die:
                    PosY += _shiftY;

                    if (PosY == 700)
                    {
                        State = DolphinState.Out;
                        break;
                    }

                    PosX += Math.Round(Math.Sin(this.PosY * 0.05) * 1.5);

                    if (_counter == DOLPHIN_DIE_UPDATES)
                    {
                        _counter = 0;
                        _currentFrame++;
                        if (_currentFrame == _imgDolphinDie.FramesCount)
                            State = DolphinState.Drops;
                    }
                    break;

                case DolphinState.Drops:
                    PosY += _shiftY;

                    if (PosY >= 700)
                    {
                        State = DolphinState.Out;
                        break;
                    }
                    PosX += Math.Round(Math.Sin(this.PosY * 0.05) * 1.5);
                    break;

                case DolphinState.Out: break;
            };
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            switch (State)
            {
                case DolphinState.Flow:
                    await jsRuntime.InvokeVoidAsync("drawSprite", 
                        _imgDolphinFlow.SpriteName, 
                        _currentFrame * _imgDolphinFlow.FrameWidth,
                        0,
                        _imgDolphinFlow.FrameWidth,
                        _imgDolphinFlow.FrameHeight,
                        PosX,
                        PosY);
                    if (Game.SHOW_BB)
                    {
                        await jsRuntime.InvokeVoidAsync("drawRect", BodyBB.PosX, BodyBB.PosY, BodyBB.Width, BodyBB.Height);
                        await jsRuntime.InvokeVoidAsync("drawRect", MouthBB.PosX, MouthBB.PosY, MouthBB.Width, MouthBB.Height);
                    }
                    break;

                case DolphinState.Eat:
                    int sPos = 0;

                    if(_currentFrame <= 3)
                    {
                        sPos = _toLeft ? (3 - _currentFrame) * _imgDolphinEat.FrameWidth : _currentFrame * _imgDolphinEat.FrameWidth;
                    }
                    else
                    {
                        sPos = _toLeft ? (_currentFrame - 3) * _imgDolphinEat.FrameWidth : (6 - _currentFrame) * _imgDolphinEat.FrameWidth;
                    }

                    await jsRuntime.InvokeVoidAsync("drawSprite",
                        _imgDolphinEat.SpriteName,
                        sPos,
                        0,
                        _imgDolphinEat.FrameWidth,
                        _imgDolphinEat.FrameHeight,
                        PosX,
                        PosY);                   
                    break;

                case DolphinState.Die:
                    await jsRuntime.InvokeVoidAsync("drawSprite",
                        _imgDolphinDie.SpriteName,
                        0,
                        _currentFrame * _imgDolphinDie.FrameHeight,
                        _imgDolphinDie.FrameWidth,
                        _imgDolphinDie.FrameHeight,
                        PosX,
                        PosY);                   
                    break;

                case DolphinState.Drops:
                    await jsRuntime.InvokeVoidAsync("drawSprite",
                       _imgDolphinDie.SpriteName,
                       0,
                       (_imgDolphinDie.FramesCount - 1) * _imgDolphinDie.FrameHeight,
                       _imgDolphinDie.FrameWidth,
                       _imgDolphinDie.FrameHeight,
                       PosX,
                       PosY);  
                    break;

                case DolphinState.Out: break;
            };
        }
    }
}
