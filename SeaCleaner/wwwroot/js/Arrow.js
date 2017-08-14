/* 
 *  объект - стрела
 */
const ARROW_SPRITE_COUNT = 10;
const ARROW_UPDATES = 4;
//
const ARROW_CATCHES = 0;
const ARROW_TURN_LEFT = 1;
const ARROW_DROPS = 2;
const ARROW_TURN_RIGHT = 3;
//
let hugPos = [[-11, 30], [8, 40], [43, 50], [89, 60], [115, 70], 
               [123, 80], [149, 90] ,[193, 100], [228, 110], [246, 120]];
//
let Arrow = {
    PosX : 145,
    PosY : 6,
    currentFrame : 9,
    counter : 0,
    state : ARROW_CATCHES,
    pic : new Image(),
    //
    loadContent : function(){
        this.pic.onload = function(){
            this.spriteWidth = this.width / ARROW_SPRITE_COUNT;
            this.spriteHeight = this.height;
        };
        this.pic.src = '/images/arrow_A.png';
    },
    initialize : function(){
        //
    },
    update : function(){
        switch(this.state){
            case ARROW_CATCHES :
                //
                break;
            case ARROW_TURN_LEFT :
                if(++this.counter >= ARROW_UPDATES){
                    this.counter = 0;
                    if(--this.currentFrame === 0){
                        this.state = ARROW_DROPS;
                        Hug.state = HUG_OPENING;
                    }                    
                    Hug.ropeLength = hugPos[this.currentFrame][1];                    
                }
                break;
            case ARROW_DROPS :
                //
                Hug.update();
                break;
            case ARROW_TURN_RIGHT : 
                if(++this.counter >= ARROW_UPDATES){
                    this.counter = 0;
                    if(++this.currentFrame === ARROW_SPRITE_COUNT - 1){
                        this.state = ARROW_CATCHES;
                        Game.isWon();
                    }
                    Hug.ropeLength = hugPos[this.currentFrame][1];                    
                }
                break;
        }
        Hug.posX = this.PosX + hugPos[this.currentFrame][0];
        Hug.posY = this.PosY; 
        Hug.update();
        
    },
    draw : function(context){
        //
        context.drawImage(
            this.pic,
            this.currentFrame * this.pic.spriteWidth,
            0,
            this.pic.spriteWidth,
            this.pic.spriteHeight,
            this.PosX,
            this.PosY,
            this.pic.spriteWidth,
            this.pic.spriteHeight);            
    }        
};

