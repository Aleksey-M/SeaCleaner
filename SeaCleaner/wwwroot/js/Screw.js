/* 
 *  ходовой винт корабля
 */
const SCREW_STOPPED = 0;
const SCREW_LEFTWARD = 1;
const SCREW_RIGHTWARD = 2;
//
const SCREW_UPDATES = 4; 
const SCREW_FRAME_COUNT = 3;
//
let Screw = {
    PosX : 60,
    PosY : 159,
    shiftY : 0,
    state : SCREW_STOPPED,
    currentFrame : 0,
    counter : 0,
    picS : new Image(),
    picL : new Image(),
    picR : new Image(),
    loadContent : function(){
        this.picS.src = '/images/screw_s.png';
        let onLoad = function(){
            this.spriteHeight = this.height;
            this.spriteWidth = this.width / SCREW_FRAME_COUNT; 
        }; 
        this.picL.onload = onLoad;
        this.picL.src = '/images/screw_l.png';
        this.picR.onload = onLoad;
        this.picR.src = '/images/screw_r.png';
    },
    initialize : function(){
        //
    },
    update : function(){
        if(this.state === SCREW_STOPPED)
            return;
        if(++this.counter > SCREW_UPDATES){
            this.counter = 0;
            if(++this.currentFrame === SCREW_FRAME_COUNT)
                this.currentFrame = 0;
        }
    },
    draw : function(context){
        switch(this.state){
            case SCREW_STOPPED :
                context.drawImage(
                    this.picS,
                    this.PosX,
                    this.PosY + this.shiftY,
                    this.picS.width,
                    this.picS.height
                );
                break;
            case SCREW_LEFTWARD : 
                context.drawImage(
                    this.picL,
                    this.currentFrame * this.picL.spriteWidth,
                    0,
                    this.picL.spriteWidth,
                    this.picL.spriteHeight,
                    this.PosX + 3,
                    this.PosY + this.shiftY,
                    this.picL.spriteWidth,
                    this.picL.spriteHeight
                );
                break;
            case SCREW_RIGHTWARD :
                context.drawImage(
                    this.picR,
                    this.currentFrame * this.picR.spriteWidth,
                    0,
                    this.picR.spriteWidth,
                    this.picR.spriteHeight,
                    this.PosX - 80,
                    this.PosY + this.shiftY,
                    this.picR.spriteWidth,
                    this.picR.spriteHeight
                );
                break;
        };
    }        
};

