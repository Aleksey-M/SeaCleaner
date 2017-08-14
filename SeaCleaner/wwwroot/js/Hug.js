/* 
 *   класс-ковш
 */
const HUG_UPDATES = 4;
const HUG_SPRITE_COUNT = 4;
//
const HUG_OPENED = 0;
const HUG_CLOSING = 1;
const HUG_CLOSED = 2;
const HUG_OPENING = 3;
//
const HUG_NO_MOVE = 0;
const HUG_MOVE_UP = 1;
const HUG_MOVE_DOWN = 2;
//
let Hug = {
    PosX : 0,
    PosY : 0,
    pic : new Image(),
    counter : 0,
    currentFrame : 3,
    state : HUG_OPENED,
    moving : HUG_NO_MOVE,
    ropeLength : 120,
    shiftY : 5,
    trashId : -1,
    bBox : new CBoundingBox(10, 15, 40, 40),
    bBoxC : new CBoundingBox(30, 40, 10, 10),  
    get posX() {
        return this.PosX;
    },
    set posX(value) {
        this.PosX = value;
        this.bBox.PosX = this.PosX + 10;
        this.bBoxC.PosX = this.PosX + 30;
    },
    get posY(){
        return this.PosY;
    },
    set posY(value){
        this.PosY = value;// + this.ropeLength;
        this.bBox.PosY = this.PosY + this.ropeLength  + 15;
        this.bBoxC.PosY = this.PosY + this.ropeLength + 40;
        //
        if(this.ropeLength <= 120 && this.state === HUG_CLOSED && Arrow.state === ARROW_CATCHES)
            Arrow.state = ARROW_TURN_LEFT;                           
    },
    loadContent : function(){
        this.pic.owner = this;  
        this.pic.onload = function(){
            this.spriteWidth = this.width / HUG_SPRITE_COUNT;
            this.spriteHeight = this.height;
            this.owner.bBox.width = this.spriteWidth - 20;
            this.owner.bBox.height = this.spriteHeight - 20;
            this.owner.bBoxC.width = this.spriteWidth - 63;
            this.owner.bBoxC.height = this.spriteHeight - 54;
        };
        this.pic.src = '/images/hug_A.png';
    },
    initialize : function(){
        //
    },
    update : function(){
        //        
        switch(this.moving){            
            case HUG_NO_MOVE :
                //
                break;
            case HUG_MOVE_UP : 
                if(Arrow.state !== ARROW_CATCHES)
                    break;
                this.ropeLength -= this.shiftY;
                if(this.ropeLength < 120 && this.state === HUG_OPENED)
                    this.ropeLength = 120;
                //
                this.bBox.PosY = this.PosY + this.ropeLength  + 15;
                this.bBoxC.PosY = this.PosY + this.ropeLength + 40;
                //                
                break;
            case HUG_MOVE_DOWN :
                if(Arrow.state !== ARROW_CATCHES)
                    break;
                this.ropeLength += this.shiftY;
                if(this.ropeLength > 640)
                    this.ropeLength = 640;
                //
                this.bBox.PosY = this.PosY + this.ropeLength  + 15;
                this.bBoxC.PosY = this.PosY + this.ropeLength + 40;
                //
                break;
        }
        //
        switch(this.state){
            case HUG_OPENED :
                //
                break;
            case HUG_CLOSING :
                if(++this.counter >= HUG_UPDATES){
                    this.counter = 0;
                    if(--this.currentFrame === 0){
                        this.state = HUG_CLOSED;
                        Game.trashes[this.trashId].isVisible = false;
                    }
                }
                break;
            case HUG_CLOSED :
                //
                break;
            case HUG_OPENING :
                if(++this.counter >= HUG_UPDATES){
                    this.counter = 0;
                    if(++this.currentFrame === HUG_SPRITE_COUNT - 1){
                        this.state = HUG_OPENED;
                        if(Arrow.state === ARROW_DROPS){
                            Arrow.state = ARROW_TURN_RIGHT;
                            Ship.addTrash(this.trashId); 
                            this.trashId = -1;
                        }
                        else{
                            Game.trashes[this.trashId].posX = this.PosX + 20;
                            Game.trashes[this.trashId].posY = this.PosY + this.ropeLength + 40;
                            Game.trashes[this.trashId].isVisible = true;
                            this.trashId = -1;
                        }                        
                    }
                }
                break;
        };
    },
    draw : function(context){
        context.beginPath();
        context.moveTo(this.PosX + 33, this.PosY + this.ropeLength + 1);
        context.lineTo(this.PosX + 33, this.PosY + 2);
        context.stroke();
        context.closePath();
        //
        context.drawImage(
            this.pic,
            this.currentFrame * this.pic.spriteWidth,
            0,
            this.pic.spriteWidth,
            this.pic.spriteHeight,
            this.PosX,
            this.PosY + this.ropeLength,
            this.pic.spriteWidth,
            this.pic.spriteHeight);       
    },
    Catch : function(){
        if(Arrow.state !== ARROW_CATCHES)
            return;
        //
        switch(this.state){
            case HUG_OPENED :
                for (let i = 0; i < Game.trashes.length; i++)
                    if(!Game.trashes[i].isCaptured && Game.trashes[i].bBox.isIntersect(this.bBoxC)){
                        //Game.trashes[i].isCaptured = true;
                        this.trashId = i;
                        this.state = HUG_CLOSING;
                        break;
                    }
                break;
            case HUG_CLOSED : 
                this.state = HUG_OPENING;
                break;
        };
    }        
};

