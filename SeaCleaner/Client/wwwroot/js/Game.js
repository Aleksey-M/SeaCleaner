//
const SCREEN_WIDTH  = 1000;
const SCREEN_HEIGHT = 700;
//
const UPDATE_INTERVAL = 25;
//
const DEPTH_Y_POS = 190; 
const DEPTH_HEIGHT = 510;
const WAVES_Y_POS = 120;
//
const GAME_PLAY = 0;
const GAME_PAUSE = 1;
const GAME_LOST = 2;
const GAME_WON = 3;
//
let Game = {
    logo      : new CSprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
    waves     : new CWaves(WAVES_Y_POS, 15, SCREEN_WIDTH),
    seaBk     : new CSprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
    corrals   : new CSprite(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
    seaTop    : new CSprite(0, DEPTH_Y_POS, SCREEN_WIDTH, DEPTH_HEIGHT),
    platePause : new CSprite(294, -420, 413, 413),
    plateWon  : new CSprite(294, -420, 413, 413),
    plateLost : new CSprite(294, -420, 413, 413),
    state : GAME_PLAY,    
    bubbles : [],  
    fishes  : [], 
    fishesId : [],
    trashes : [],
    dolphins : [],
    //
    timerId: -1,       
    //
    loadContent : function(){
        //
        this.logo.loadSprite("/images/Logo.jpg");
        //MultiImage.loadImages();
        this.seaBk.loadSprite("/images/sea_bk.jpg");
        this.corrals.loadSprite("/images/corrals.png");
        this.seaTop.loadSprite("/images/sea_top.png");
        this.waves.LoadSprite(5, 7, '/images/wave_bk.png', '/images/wave_top.png');
        this.plateLost.loadSprite("/images/lost.png");
        this.platePause.loadSprite("/images/pause.png");
        this.plateWon.loadSprite("/images/won.png");
        //
        for(let i = 0; i< 20; i++)
            this.bubbles.push(new CBubble(0, 0, 5, -2, 1));
        //
        let b = false;
        let fNames = ['fish_1_l', 'fish_1_r', 'fish_2_l',
                      'fish_2_r', 'fish_3_l', 'fish_3_r'];
        for(let i=0; i< fNames.length; i++)
            this.fishes.push(new CFishes(5, 5, b = !b, fNames[i]));
        //
        let tNames = ['trash_1', 'trash_2', 'trash_3', 'trash_4', 'trash_5'];
        for(let i=0; i< 5; i++){
            this.trashes.push(new CTrash(tNames[i]));
            this.trashes.push(new CTrash(tNames[i]));
        }   
        //
        b = false;
        for (let i =0; i< 4; i++)
            this.dolphins.push(new CDolphin(b = !b));
        //
        Ship.loadContent();        
    },
    //
    initialize  : function(canvas){
        this.context = canvas.getContext('2d');
        this.logo.draw(this.context);
        //this.context.lineWidth = 2;
        //
        for (let i = 0; i < 20; i++){
            this.bubbles[i].PosX = Math.random()*1000;
            this.bubbles[i].PosY = Math.random() * 1000 % 550; /*+ 550*/
        }            
        for (let i = 0; i < 6; i++)
            this.fishes[i].initialize();
        this.fishesId.push(0);
        this.fishesId.push(5);
        //
        for (let i = 0; i < this.trashes.length; i++)
            this.trashes[i].initialize();
        //
        for (let i=0; i< 4; i++)
            this.dolphins[i].initialize();
        //       
        Ship.initialize();             
    },
    //
    update : function(){
        if(this.state === GAME_LOST){
            if (this.plateLost.PosY < 0)
                this.plateLost.PosY += 30;
            else
                this.onGameFinish();
            return;
        }
        if(this.state === GAME_WON){
            if(this.plateWon.PosY < 0)
                this.plateWon.PosY += 30;
            else
                this.onGameFinish();
            return;
        }
        if(this.state === GAME_PAUSE){
            if(this.platePause.PosY < 0)
                this.platePause.PosY += 30;
            return;
        }
        if(this.state === GAME_PLAY && this.platePause.PosY > -420){
            this.platePause.PosY -= 30;
            return;
        }        
        this.waves.Update();
        //
        for (let i=0; i<20; i++)
            this.bubbles[i].update();
        //
        for (let i =0; i< 2; i++)
            this.fishes[this.fishesId[i]].update();
        //
        if(this.fishes[this.fishesId[0]].isOffScreen()){
            this.fishes[this.fishesId[0]].initialize();
            this.fishesId[0] = Math.round(Math.random()*1000) % 6;
            while(this.fishesId[0] === this.fishesId[1])
                this.fishesId[0] = Math.round(Math.random()*1000) % 6;            
        }
        if(this.fishes[this.fishesId[1]].isOffScreen()){
            this.fishes[this.fishesId[1]].initialize();
            this.fishesId[1] = Math.round(Math.random()*1000) % 6;
            while(this.fishesId[0] === this.fishesId[1])
                this.fishesId[1] = Math.round(Math.random()*1000) % 6;            
        }
        //
        for (let i = 0; i < this.trashes.length; i++)
            this.trashes[i].update();
        //
        for (let i=0; i< 4; i++)
            this.dolphins[i].update();
        //
        for (let i = 0; i<4; i++)
            if(this.dolphins[i].state === DOLPHIN_FLOW)
                for (let j = 0; j < 10; j++)
                    if (!this.trashes[j].isCaptured 
                        && this.dolphins[i].mouthBB.isIntersect(this.trashes[j].bBox)){
                         this.trashes[j].isCaptured = true;
                         this.dolphins[i].trashId = j;
                         this.dolphins[i].setState(DOLPHIN_EAT);
                         this.onDolphinDie(this);
                         this.onTrashLoad(this);
                         //break;
                     }
        //
        for (let i = 0; i < 4; i++)
            if (this.dolphins[i].state === DOLPHIN_FLOW &&
                this.dolphins[i].bodyBB.isIntersect(Hug.bBox)) {
                this.dolphins[i].setState(DOLPHIN_DIE);
                this.onDolphinDie(this);
            }
        Ship.update();
    },
    //
    draw : function(){
        //
        //this.context.clearRect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        this.seaBk.draw(this.context);
        this.waves.drawBk(this.context);
        //
        for (let i =0; i< 2; i++)
            this.fishes[this.fishesId[i]].draw(this.context);
        //
        this.corrals.draw(this.context);
        //
        for (let i = 0; i < this.trashes.length; i++)
            this.trashes[i].draw(this.context);
        //   
        for (let i=0; i< 4; i++)
            this.dolphins[i].draw(this.context);
        //
        Ship.draw(this.context);
        //
        for (let i=0; i<20; i++)
            this.bubbles[i].draw(this.context);
        //       
        this.seaTop.draw(this.context);
        this.waves.drawTop(this.context);
        //
        switch(this.state){
            case GAME_PLAY :
                if(this.platePause.PosY > -420)
                    this.platePause.draw(this.context);
                break;
            case GAME_PAUSE :
                this.platePause.draw(this.context);
                break;
            case GAME_WON :
                this.plateWon.draw(this.context);
                break;
            case GAME_LOST :
                this.plateLost.draw(this.context);
                break;    
        }       
    },
    liveDolphins : function(){
        // проверка, есть ли хоть один плавающий дельфин
        for (let i =0; i< this.dolphins.length; i++)
            if(this.dolphins[i].state === DOLPHIN_FLOW)
                return true;
        return false;
    },       
    dieDolphins : function(){
        // проверка, все ли дельфины утонули
        for (let i =0; i< this.dolphins.length; i++)
            if(this.dolphins[i].state !== DOLPHIN_OUT)
                return false;
        return true;
    },   
    thereIsTrash : function(){
        for (let i = 0; i < this.trashes.length; i++)
            if(!this.trashes[i].isCaptured)
                return true;
        return false;
    },        
    isLost : function(){
        if(this.dieDolphins()){
            this.state = GAME_LOST;
            return true;            
        }
        else
            return false;
    },
    isWon : function(){
        if(this.liveDolphins() && !this.thereIsTrash()){
            this.state = GAME_WON;
            return true;
        }
        else
            return false;        
    },        
    //
    mainLoop : function(){
                
        this.update();
        this.draw();        
    },
    onKeyDown : function(event){
        switch(event.keyCode){
            case 37 : // left
                Screw.state = SCREW_LEFTWARD;
                break;
            case 39 : // right
                Screw.state = SCREW_RIGHTWARD;
                break;           
            case 38 : 
                Hug.moving = HUG_MOVE_UP;
                break;
            case 40 :
                Hug.moving = HUG_MOVE_DOWN;
                break;
            case 32 : 
                Hug.Catch();
                break;
            case 80 :
                if(Game.state === GAME_PLAY)
                    Game.state = GAME_PAUSE;
                else
                    if(Game.state === GAME_PAUSE)
                        Game.state = GAME_PLAY;
                break;                 
        }
    },
    //
    onKeyUp : function(event){
        switch(event.keyCode){
            case 37 :
                if(Screw.state === SCREW_LEFTWARD)
                    Screw.state = SCREW_STOPPED;
                break;
            case 39 :
                if(Screw.state === SCREW_RIGHTWARD)
                    Screw.state = SCREW_STOPPED;
                break;
            case 38 :
                if(Hug.moving === HUG_MOVE_UP)
                    Hug.moving = HUG_NO_MOVE;
                break;
            case 40 :
                if(Hug.moving === HUG_MOVE_DOWN)
                    Hug.moving = HUG_NO_MOVE;
                break;             
        }
    },        
    getLiveDolphinsCount: function () {
        let dolphinsCount = 0;
        for (let i = 0; i < this.dolphins.length; i++) {
            if (this.dolphins[i].state === DOLPHIN_FLOW)
                dolphinsCount++;
        }
        return dolphinsCount;
    },
    getTrashesInWater: function () {
        let trashesCount = 0;
        for (let i = 0; i < this.trashes.length; i++) {
            if (!this.trashes[i].isCaptured)
                trashesCount++;
        }
        return trashesCount;
    },
    startTime: null,
    isGameFinished: false,
    onGameFinish: function () {
        if (this.isGameFinished) return;

        let dolphinsCount = 0;
        for (let i = 0; i < this.dolphins.length; i++) {
            if (this.dolphins[i].state === DOLPHIN_FLOW)
                dolphinsCount++;
        }

        let totalSeconds = Math.floor((Date.now() - this.startTime) / 1000);


        this.onGameEndCallback({
            victory: dolphinsCount > 0,
            dolphins: dolphinsCount,
            seconds: totalSeconds
        });
        this.isGameFinished = true;
    },
    onGameEndCallback: function () { },
    start: function (_onGameEnd) {
        this.startTime = Date.now();
        this.timerId = setInterval('Game.mainLoop()', UPDATE_INTERVAL);
        window.addEventListener('keydown', Game.onKeyDown, true);
        window.addEventListener('keyup', Game.onKeyUp, true);
        this.onGameEndCallback = _onGameEnd;
    },
    onDolphinDie: function () { },
    onTrashLoad: function () { }
};

