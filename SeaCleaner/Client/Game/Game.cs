using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeaCleaner.Client.Game
{
    internal enum GameState { GAME_PLAY, GAME_PAUSE, GAME_LOST, GAME_WON }

    internal class Game
    {
        public const int SCREEN_WIDTH = 1000;
        public const int SCREEN_HEIGHT = 700;
        public const int UPDATE_INTERVAL = 5;
        public const int DEPTH_Y_POS = 190;
        public const int DEPTH_HEIGHT = 510;
        public const int WAVES_Y_POS = 120;
        public const bool SHOW_BB = false;

        private readonly IJSRuntime _jsRuntime;
        private readonly string _canvasId;
        public Dictionary<string, SpriteImageInfo> Sprites;
        private Timer _timer;
        private DateTimeOffset _startTime;
        private readonly Random _random = new Random();

        public GameState State { get; private set; }

        private Sprite _logo;
        private Sprite _seaBack;
        private Sprite _seaFront;
        private Sprite _corrals;
        private Sprite _platePause;
        private Sprite _plateWon;
        private Sprite _plateLost;
        private Waves _waves;

        private readonly List<Bubble> _bubbles = new List<Bubble>();
        private readonly List<Fishes> _fishes = new List<Fishes>();
        public List<Trash> Trashes { get; } = new List<Trash>();
        private int _activeFishesIndex1 = 0;
        private int _activeFishesIndex2 = 5;

        private readonly List<Dolphin> _dolphins = new List<Dolphin>();
        private Ship _ship;        

        public Game(IJSRuntime jsRuntime, string canvasId)
        {
            _jsRuntime = jsRuntime;
            _canvasId = canvasId;
            State = GameState.GAME_PLAY;
        }

        public async ValueTask LoadResources()
        {
            Sprites = await GameResources.LoadImages(_jsRuntime);
        }

        public async ValueTask Initialize()
        {
            await _jsRuntime.InvokeVoidAsync("initializeScreen", _canvasId);
            _logo = new Sprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, Sprites["Logo"]);
            await _logo.Draw(_jsRuntime);

            _seaBack = new Sprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, Sprites["SeaBack"]);
            _seaFront = new Sprite(0, DEPTH_Y_POS, SCREEN_WIDTH, SCREEN_HEIGHT, Sprites["SeaFront"]);
            _corrals = new Sprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, Sprites["Corrals"]);
            _platePause = new Sprite(294, -420, 413, 413, Sprites["PlatePause"]);
            _plateWon = new Sprite(294, -420, 413, 413, Sprites["PlateWon"]);
            _plateLost = new Sprite(294, -420, 413, 413, Sprites["PlateLost"]);

            _waves = new Waves(WAVES_Y_POS, 15, SCREEN_WIDTH + 100, 5, Sprites["WavesFront"], Sprites["WavesBack"]);

            for (int i = 0; i < 20; i++)
            {
                _bubbles.Add(new Bubble(_random.NextDouble() * SCREEN_WIDTH, _random.NextDouble() * SCREEN_WIDTH % 550, 5, -2, 1, Sprites["Bubble"]));
            }

            bool b = false;
            string[] fNames = new[] { "Fish1L", "Fish1R", "Fish2L", "Fish2R", "Fish3L", "Fish3R" };
            for (int i = 0; i < fNames.Length; i++)
            {
                _fishes.Add(new Fishes(5, 5, b = !b, Sprites[fNames[i]]));
                _fishes.Last().Initialize();
            }

            string[] tNames = new[] { "Trash1", "Trash2", "Trash3", "Trash4", "Trash5" };
            for (int i = 0; i < 5; i++)
            {
                Trashes.Add(new Trash(Sprites[tNames[i]]));
                Trashes.Add(new Trash(Sprites[tNames[i]]));
            }
            _trashesCount = Trashes.Count;

            b = false;
            Dolphin._dPos = 0;
            for (int i = 0; i < 4; i++)
            {
                b = !b;
                var d = new Dolphin(b,
                    Sprites["DolphinFlow" + (b ? "L" : "R")],
                    Sprites["DolphinEat" + (b ? "L" : "R")],
                    Sprites["DolphinDie" + (b ? "L" : "R")],
                    CheckLost, CheckWon);
                d.Initialize();
                _dolphins.Add(d);
            }
            _dolphinsCount = _dolphins.Count;

            _ship = new Ship(this);
        }

        public async ValueTask Update()
        {
            if (State == GameState.GAME_PAUSE && _platePause.PosY >= 0) return;

            if (State == GameState.GAME_LOST)
            {
                if (_plateLost.PosY < 0)
                    _plateLost.PosY += 30;
                else
                    await EndGame();
                return;
            }

            if (State == GameState.GAME_WON)
            {
                if (_plateWon.PosY < 0)
                    _plateWon.PosY += 30;
                else
                    await EndGame();
                return;
            }

            if (State == GameState.GAME_PAUSE)
            {
                if (_platePause.PosY < 0)
                    _platePause.PosY += 30;
                return;
            }

            if (State == GameState.GAME_PLAY && _platePause.PosY > -420)
            {
                _platePause.PosY -= 30;
                return;
            }

            _waves.Update();

            for (int i = 0; i < 20; i++) _bubbles[i].Update();

            _fishes[_activeFishesIndex1].Update();
            _fishes[_activeFishesIndex2].Update();

            if (_fishes[_activeFishesIndex1].IsOffScreen())
            {
                _fishes[_activeFishesIndex1].Initialize();
                _activeFishesIndex1 = _random.Next(0, 6);
                if (_activeFishesIndex1 == _activeFishesIndex2) _activeFishesIndex1 = (_activeFishesIndex1 + 1) % 6;
            }

            if (_fishes[_activeFishesIndex2].IsOffScreen())
            {
                _fishes[_activeFishesIndex2].Initialize();
                _activeFishesIndex2 = _random.Next(0, 6);
                if (_activeFishesIndex2 == _activeFishesIndex1) _activeFishesIndex2 = (_activeFishesIndex2 + 1) % 6;
            }

            foreach(var t in Trashes)
            {
                t.Update();
            }

            foreach (var d in _dolphins)
            {
                d.Update();

                if (d.State == DolphinState.Flow)
                {
                    foreach (var t in Trashes)
                    {
                        if (t.IsCaptured) continue;

                        if (d.MouthBB.IsIntersect(t.BBox))
                        {
                            t.IsCaptured = true;
                            d.CapturedTrash = t;
                            d.State = DolphinState.Eat;
                            break;
                        }
                    }
                }
            }

            foreach(var d in _dolphins)
            {
                if(d.State == DolphinState.Flow && d.BodyBB.IsIntersect(_ship.Arrow.Hug.HugOuterBBox))
                {
                    d.State = DolphinState.Die;
                }
            }

            _ship.Update();
        }

        public async ValueTask Draw()
        {
            if (State == GameState.GAME_PAUSE && _platePause.PosY >= 0) return;

            await _seaBack.Draw(_jsRuntime);
            await _waves.DrawBack(_jsRuntime);

            await _fishes[_activeFishesIndex1].Draw(_jsRuntime);
            await _fishes[_activeFishesIndex2].Draw(_jsRuntime);
            await _corrals.Draw(_jsRuntime);
            foreach(var t in Trashes)
            {
                await t.Draw(_jsRuntime);
            }

            foreach(var d in _dolphins)
            {
                await d.Draw(_jsRuntime);
            }

            await _ship.Draw(_jsRuntime);

            foreach (var b in _bubbles)
            {
                await b.Draw(_jsRuntime);
            }

            await _seaFront.Draw(_jsRuntime);
            await _waves.DrawFront(_jsRuntime);

            switch (State)
            {
                case GameState.GAME_PLAY:
                    if (_platePause.PosY > -420) await _platePause.Draw(_jsRuntime);
                    break;
                case GameState.GAME_PAUSE:
                    await _platePause.Draw(_jsRuntime);
                    break;
                case GameState.GAME_WON:
                    await _plateWon.Draw(_jsRuntime);
                    break;
                case GameState.GAME_LOST:
                    await _plateLost.Draw(_jsRuntime);
                    break;
            }
        }

        public async ValueTask MainLoop()
        {
            await Update();
            await Draw();
        }

        [JSInvokable]
        public void OnKeyDown(int keyCode)
        {
            if(keyCode == 80)
            {
                if (State == GameState.GAME_PLAY)
                    State = GameState.GAME_PAUSE;
                else
                    if (State == GameState.GAME_PAUSE)
                        State = GameState.GAME_PLAY;
            }

            _ship.KeyDown(keyCode);
        }

        [JSInvokable]
        public void OnKeyUp(int keyCode)
        {
            _ship.KeyUp(keyCode);
        }

        private Action<int> _onDolphinDie;
        private Action<int> _onTrashCatched;
        private Func<bool, int, long, ValueTask> _onGameEnd;
        private Action _afterGameFinished;

        private DotNetObjectReference<Game> _gameDotNetReference;

        public async ValueTask StartGame(Action<int> onDolphinDie, Action<int> onTrashCatched, Func<bool, int, long, ValueTask> onGameEnd, Action afterGameFinished)
        {
            _onDolphinDie = onDolphinDie;
            _onTrashCatched = onTrashCatched;
            _onGameEnd = onGameEnd;
            _afterGameFinished = afterGameFinished;

            _gameDotNetReference = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("setupKeyEvents", _gameDotNetReference);

            _startTime = DateTimeOffset.Now;
            _timer = new Timer(async _ => await MainLoop(), null, 500, UPDATE_INTERVAL);
        }

        private async ValueTask EndGame()
        {
            if(_timer != null) await _timer.DisposeAsync();
            await _jsRuntime.InvokeVoidAsync("clearKeyEvents");
            await _jsRuntime.InvokeVoidAsync("clearResources");

            Thread.Sleep(1500);
            
            _gameDotNetReference?.Dispose();            

            int dCount = _dolphins.Count(d => d.State == DolphinState.Flow);
            long seconds = Convert.ToInt64((DateTimeOffset.Now - _startTime).TotalSeconds);
            bool victory = dCount > 0;
            await _onGameEnd(victory, dCount, seconds);            

            _afterGameFinished();
        }

        private int _dolphinsCount;
        private int _trashesCount;

        public void CheckLost()
        {
            int c = _dolphins.Count(d => d.State == DolphinState.Flow);
            if(c < _dolphinsCount)
            {
                _dolphinsCount = c;
                _onDolphinDie(c);
            }

            if(c == 0)
            {
                State = GameState.GAME_LOST;
            }
        }

        public void CheckWon()
        {
            int c = Trashes.Count(t => !t.IsCaptured);
            if(c < _trashesCount)
            {
                _trashesCount = c;
                _onTrashCatched(c);
            }

            if(c == 0 && _dolphinsCount > 0)
            {
                State = GameState.GAME_WON;
            }
        }
    }
}
