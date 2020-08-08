using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal class Ship
    {
        public double PosX { get; private set; } = 20;
        public double PosY { get; private set; } = 24;
        private readonly double _shiftX = 6;
        private double _shiftY = 0;
        private readonly SpriteImageInfo _shipImage;
        private int _fCounter = 0;
        private readonly double _fluctCoef = 0.15;
        private readonly double _fluctuation = 3.0;
        private int _counter = 0;
        private readonly List<Trash> _trashes = new List<Trash>();

        private readonly Screw _screw;
        public Arrow Arrow { get; }
        private readonly Game _game;

        public void CheckGameLost() => _game.CheckLost();
        public void CheckGameWon() => _game.CheckWon();

        public Ship(Game game)
        {
            _game = game;
            _shipImage = _game.Sprites["Ship"];
            _screw = new Screw(_game.Sprites["ScrewS"], _game.Sprites["ScrewL"], _game.Sprites["ScrewR"]);
            Arrow = new Arrow(_game.Sprites["Arrow"], this, _game.Sprites["Hug"]);
        }

        public void Update()
        {
            if (++_counter > 1)
            {
                _counter = 0;
                if (++_fCounter >= 1800)
                    _fCounter = 0;
            }

            // куда плывет корабль - зависит от винта :)
            double d = 0;
            switch (_screw.State)
            {
                case ScrewState.SCREW_STOPPED:
                    //
                    break;
                case ScrewState.SCREW_LEFTWARD:
                    PosX -= _shiftX;
                    if (PosX < -300)
                        PosX = -300;
                    else
                        d = -_shiftX;
                    break;
                case ScrewState.SCREW_RIGHTWARD:
                    PosX += _shiftX;
                    if (PosX > 850)
                        PosX = 850;
                    else
                        d = _shiftX;
                    break;
            }

            _shiftY = Math.Sin(_fCounter * _fluctCoef) * _fluctuation;

            for (int i = 0; i < _trashes.Count; i++)
            {
                _trashes[i].PosX += d;

                if (i <= 3)
                {
                    _trashes[i].PosY = PosY + 80 + _shiftY;
                }
                else if (i >= 4 && i <= 6)
                {
                    _trashes[i].PosY = PosY + 68 + _shiftY;
                }
                else if (i == 7 || i == 8)
                {
                    _trashes[i].PosY = PosY + 56 + _shiftY;
                }
                else if (i == 9)
                {
                    _trashes[i].PosY = PosY + 44 + _shiftY;
                }
            }

            _screw.PosX = PosX + 40;
            _screw.PosY = PosY + 135 + _shiftY;
            _screw.Update();

            Arrow.PosX = PosX + 125;
            Arrow.PosY = PosY - 18 + _shiftY;
            Arrow.Update();
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            await Arrow.Draw(jsRuntime);

            await jsRuntime.InvokeVoidAsync("drawImage",
                _shipImage.SpriteName,
                PosX,
                PosY + _shiftY,
                _shipImage.FrameWidth,
                _shipImage.FrameHeight);

            await Arrow.Hug.Draw(jsRuntime);
            await _screw.Draw(jsRuntime);
        }

        public void KeyDown(int keyCode)
        {
            switch (keyCode)
            {
                case 37: // left
                    _screw.State = ScrewState.SCREW_LEFTWARD;
                    break;
                case 39: // right
                    _screw.State = ScrewState.SCREW_RIGHTWARD;
                    break;
                case 38:
                    Arrow.Hug.Moving = HugMoving.HUG_MOVE_UP;
                    break;
                case 40:
                    Arrow.Hug.Moving = HugMoving.HUG_MOVE_DOWN;
                    break;
                case 32:
                    Arrow.Hug.TryCatchTrash(_game.Trashes);
                    break;
                default: break;
            }
        }

        public void KeyUp(int keyCode)
        {
            switch (keyCode)
            {
                case 37:
                    if (_screw.State == ScrewState.SCREW_LEFTWARD)
                        _screw.State = ScrewState.SCREW_STOPPED;
                    break;
                case 39:
                    if (_screw.State == ScrewState.SCREW_RIGHTWARD)
                        _screw.State = ScrewState.SCREW_STOPPED;
                    break;
                case 38:
                    if (Arrow.Hug.Moving == HugMoving.HUG_MOVE_UP)
                        Arrow.Hug.Moving = HugMoving.HUG_NO_MOVE;
                    break;
                case 40:
                    if (Arrow.Hug.Moving == HugMoving.HUG_MOVE_DOWN)
                        Arrow.Hug.Moving = HugMoving.HUG_NO_MOVE;
                    break;
                default: break;
            }
        }

        public void AddTrash(Trash trash)
        {
            _trashes.Add(trash);

            if (_trashes.Count <= 4)
            {
                trash.PosX = PosX + 100 + 23 * _trashes.Count;
                trash.PosY = PosY + 80;
            }
            else if (_trashes.Count >= 5 && _trashes.Count <= 7)
            {
                trash.PosX = PosX + 111 + 23 * (_trashes.Count % 4);
                trash.PosY = PosY + 68;
            }
            else if (_trashes.Count == 8 || _trashes.Count == 9)
            {
                trash.PosX = PosX + 122 + 23 * (_trashes.Count % 7);
                trash.PosY = PosY + 56;
            }
            else if (_trashes.Count == 10)
            {
                trash.PosX = PosX + 157;
                trash.PosY = PosY + 44;
            }

            trash.IsCaptured = true;
            trash.IsVisible = true;
            //Game.onTrashLoad(Game);
        }
    }

    internal enum ScrewState { SCREW_STOPPED, SCREW_LEFTWARD, SCREW_RIGHTWARD }

    internal class Screw
    {
        const int SCREW_UPDATES = 4;
        const int SCREW_FRAME_COUNT = 3;

        public double PosX { get; set; } = 60;
        public double PosY { get; set; } = 159;
        public ScrewState State { get; set; } = ScrewState.SCREW_STOPPED;
        private int _currentFrame = 0;
        private int _counter = 0;
        private readonly SpriteImageInfo _picS;
        private readonly SpriteImageInfo _picL;
        private readonly SpriteImageInfo _picR;

        public Screw(SpriteImageInfo picS, SpriteImageInfo picL, SpriteImageInfo picR)
        {
            _picS = picS;
            _picL = picL;
            _picR = picR;
        }

        public void Update()
        {
            if (State == ScrewState.SCREW_STOPPED) return;

            if (++_counter > SCREW_UPDATES)
            {
                _counter = 0;
                if (++_currentFrame == SCREW_FRAME_COUNT)
                    _currentFrame = 0;
            }
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            switch (State)
            {
                case ScrewState.SCREW_STOPPED:
                    await jsRuntime.InvokeVoidAsync("drawImage",
                        _picS.SpriteName,
                        PosX,
                        PosY,
                        _picS.FrameWidth,
                        _picS.FrameHeight);
                    break;

                case ScrewState.SCREW_LEFTWARD:
                    await jsRuntime.InvokeVoidAsync("drawSprite",
                        _picL.SpriteName,
                        _currentFrame * _picL.FrameWidth,
                        0,
                        _picL.FrameWidth,
                        _picL.FrameHeight,
                        PosX + 3,
                        PosY);
                    break;

                case ScrewState.SCREW_RIGHTWARD:
                    await jsRuntime.InvokeVoidAsync("drawSprite",
                        _picR.SpriteName,
                        _currentFrame * _picR.FrameWidth,
                        0,
                        _picR.FrameWidth,
                        _picR.FrameHeight,
                        PosX - 80,
                        PosY);
                    break;
            }
        }
    }

    internal enum ArrowState { ARROW_CATCHES, ARROW_TURN_LEFT, ARROW_DROPS, ARROW_TURN_RIGHT }

    internal class Arrow
    {
        const int ARROW_UPDATES = 4;
        private readonly List<(double x, double y)> _hugPos = new List<(double x, double y)> {
            (-11, 30), (8, 40), (43, 50), (89, 60), (115, 70),
            (123, 80), (149, 90), (193, 100), (228, 110), (246, 120)};

        public double PosX { get; set; } = 145;
        public double PosY { get; set; } = 6;
        private int _currentFrame = 9;
        private int _counter = 0;
        public ArrowState State { get; set; } = ArrowState.ARROW_CATCHES;
        private readonly SpriteImageInfo _arrowImg;
        public Ship Ship { get; }
        public Hug Hug { get; }

        public Arrow(SpriteImageInfo arrowImg, Ship ship, SpriteImageInfo hugImg)
        {
            _arrowImg = arrowImg;
            Ship = ship;

            Hug = new Hug(this, hugImg);
        }

        public void Update()
        {
            switch (State)
            {
                case ArrowState.ARROW_CATCHES:
                    break;

                case ArrowState.ARROW_TURN_LEFT:
                    if (++_counter >= ARROW_UPDATES)
                    {
                        _counter = 0;
                        if (--_currentFrame == 0)
                        {
                            State = ArrowState.ARROW_DROPS;
                            Hug.State = HugState.HUG_OPENING;
                        }
                        Hug.RopeLength = _hugPos[_currentFrame].y;
                    }
                    break;

                case ArrowState.ARROW_DROPS:
                    Hug.Update();
                    break;

                case ArrowState.ARROW_TURN_RIGHT:
                    if (++_counter >= ARROW_UPDATES)
                    {
                        _counter = 0;
                        if (++_currentFrame == _arrowImg.FramesCount - 1)
                        {
                            State = ArrowState.ARROW_CATCHES;
                            Ship.CheckGameLost();
                            Ship.CheckGameWon();
                        }
                        Hug.RopeLength = _hugPos[_currentFrame].y;
                    }
                    break;
            }
            Hug.PosX = PosX + _hugPos[_currentFrame].x;
            Hug.PosY = PosY;
            Hug.Update();
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync("drawSprite",
                _arrowImg.SpriteName,
                _currentFrame * _arrowImg.FrameWidth,
                0,
                _arrowImg.FrameWidth,
                _arrowImg.FrameHeight,
                PosX,
                PosY);
        }
    }

    internal enum HugState { HUG_OPENED, HUG_CLOSING, HUG_CLOSED, HUG_OPENING }
    internal enum HugMoving { HUG_NO_MOVE, HUG_MOVE_UP, HUG_MOVE_DOWN }

    internal class Hug
    {
        const int HUG_UPDATES = 4;
        private double _posX;
        private double _posY;
        private readonly SpriteImageInfo _hugImg;
        private int _counter = 0;
        private int _currentFrame = 3;
        public HugState State { get; set; } = HugState.HUG_OPENED;
        public HugMoving Moving { get; set; } = HugMoving.HUG_NO_MOVE;
        public double RopeLength { get; set; } = 120;
        private readonly double _shiftY = 5;
        public Trash CatchedTrash;
        public BoundingBox HugOuterBBox { get;  }
        private readonly BoundingBox _hugInnerBBox;

        public double PosX
        {
            get => _posX;
            set
            {
                _posX = value;
                HugOuterBBox.PosX = _posX + 10;
                _hugInnerBBox.PosX = _posX + 30;
            }
        }

        public double PosY
        {
            get => _posY;
            set
            {
                _posY = value;
                HugOuterBBox.PosY = _posY + RopeLength + 15;
                _hugInnerBBox.PosY = _posY + RopeLength + 40;
                //
                if (RopeLength <= 120 && State == HugState.HUG_CLOSED && _arrow.State == ArrowState.ARROW_CATCHES)
                    _arrow.State = ArrowState.ARROW_TURN_LEFT;
            }
        }

        private readonly Arrow _arrow;

        public Hug(Arrow arrow, SpriteImageInfo hugImg)
        {
            _hugImg = hugImg;
            _arrow = arrow;

            HugOuterBBox = new BoundingBox(10, 15, _hugImg.FrameWidth - 20, _hugImg.FrameHeight - 20);
            _hugInnerBBox = new BoundingBox(30, 40, _hugImg.FrameWidth - 63, _hugImg.FrameHeight - 54);
        }

        public void Update()
        {
            switch (Moving)
            {
                case HugMoving.HUG_NO_MOVE:
                    break;

                case HugMoving.HUG_MOVE_UP:
                    if (_arrow.State != ArrowState.ARROW_CATCHES) break;

                    RopeLength -= _shiftY;
                    if (RopeLength < 120 && State == HugState.HUG_OPENED) RopeLength = 120;

                    HugOuterBBox.PosY = _posY + RopeLength + 15;
                    _hugInnerBBox.PosY = _posY + RopeLength + 40;
                    break;

                case HugMoving.HUG_MOVE_DOWN:
                    if (_arrow.State != ArrowState.ARROW_CATCHES) break;

                    RopeLength += _shiftY;
                    if (RopeLength > 640) RopeLength = 640;

                    HugOuterBBox.PosY = _posY + RopeLength + 15;
                    _hugInnerBBox.PosY = _posY + RopeLength + 40;
                    break;
            }

            switch (State)
            {
                case HugState.HUG_OPENED:
                    break;

                case HugState.HUG_CLOSING:
                    if (++_counter >= HUG_UPDATES)
                    {
                        _counter = 0;
                        if (--_currentFrame == 0)
                        {
                            State = HugState.HUG_CLOSED;
                            CatchedTrash.IsVisible = false;
                        }
                    }
                    break;

                case HugState.HUG_CLOSED:
                    break;

                case HugState.HUG_OPENING:
                    if (++_counter >= HUG_UPDATES)
                    {
                        _counter = 0;
                        if (++_currentFrame == _hugImg.FramesCount - 1)
                        {
                            State = HugState.HUG_OPENED;
                            if (_arrow.State == ArrowState.ARROW_DROPS)
                            {
                                _arrow.State = ArrowState.ARROW_TURN_RIGHT;
                                _arrow.Ship.AddTrash(CatchedTrash);
                                CatchedTrash = null;
                            }
                            else
                            {
                                CatchedTrash.PosX = PosX + 20;
                                CatchedTrash.PosY = PosY + RopeLength + 40;
                                CatchedTrash.IsVisible = true;
                                CatchedTrash.IsCaptured = false;
                                CatchedTrash = null;
                            }
                        }
                    }
                    break;
            };
        }

        public async ValueTask Draw(IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync("drawLine",
                PosX + 33,
                PosY + RopeLength + 1,
                PosX + 33,
                PosY + 2);

            await jsRuntime.InvokeVoidAsync("drawSprite",
                _hugImg.SpriteName,
                _currentFrame * _hugImg.FrameWidth,
                0,
                _hugImg.FrameWidth,
                _hugImg.FrameHeight,
                PosX,
                PosY + RopeLength);

            if (Game.SHOW_BB)
            {
                await jsRuntime.InvokeVoidAsync("drawRect", HugOuterBBox.PosX, HugOuterBBox.PosY, HugOuterBBox.Width, HugOuterBBox.Height);
                await jsRuntime.InvokeVoidAsync("drawRect", _hugInnerBBox.PosX, _hugInnerBBox.PosY, _hugInnerBBox.Width, _hugInnerBBox.Height);
            }
        }

        public void TryCatchTrash(List<Trash> allTrashes)
        {
            if (_arrow.State != ArrowState.ARROW_CATCHES) return;
            //
            switch (State)
            {
                case HugState.HUG_OPENED:
                    foreach (var t in allTrashes)
                    {
                        if (!t.IsCaptured && t.BBox.IsIntersect(_hugInnerBBox))
                        {
                            CatchedTrash = t;
                            t.IsCaptured = true;
                            State = HugState.HUG_CLOSING;
                            break;
                        }
                    }
                    break;

                case HugState.HUG_CLOSED:
                    State = HugState.HUG_OPENING;
                    break;
            };
        }
    }
}
