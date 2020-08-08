/* 
 * Класс Дельфин
 */
const DOLPHIN_FLOW_UPDATES = 6;
const DOLPHIN_EAT_UPDATES = 5;
const DOLPHIN_DIE_UPDATES = 7;
//
const DOLPHIN_FLOW = 0;
const DOLPHIN_EAT = 1;
const DOLPHIN_DIE = 2;
const DOLPHIN_DROPS = 3;
const DOLPHIN_OUT = 4;
//
let DPos = 0;
//
function CDolphin(_ToLeft){
    this.toLeft = _ToLeft;
    this.shiftX = 2;
    this.shiftY = 2; //
    this.state = DOLPHIN_FLOW;
    this.counter = 0;
    this.currentFrame = 0;
    this.trashId = -1;
    //
    this.initialize = function(){
        if(this.toLeft){
            this.flowSN = 'dolphin_flow_l';
            this.eatSN = 'dolphin_eat_l';
            this.dieSN = 'dolphin_die_l';
            this.shiftX = -this.shiftX;
            //
            this.PosX = 1000 + DPos;/* + Math.round(Math.random() * 1000) % 500;*/
            DPos += 500;
            this.PosY = Math.round(Math.random() * 1000) % 480 + 200;
            //
            this.bodyBB = new CBoundingBox(this.PosX, this.PosY + 10, 
                MultiImage[this.flowSN].SpriteWidth - 20, MultiImage[this.flowSN].SpriteHeight - 15);
            this.mouthBB = new CBoundingBox(this.PosX, this.PosY + 40, 20, 25);   
            //
            this.currentFrame = MultiImage[this.flowSN].SpriteCount - 1;
        }
        else{
            this.flowSN = 'dolphin_flow_r';
            this.eatSN = 'dolphin_eat_r';
            this.dieSN = 'dolphin_die_r';
            //
            this.PosX = -150 - DPos;/*Math.round(Math.random() * 1000) % 500;*/
            DPos += 500;
            this.PosY = Math.round(Math.random() * 1000) % 480 + 200;
            //
            this.bodyBB = new CBoundingBox(this.PosX, this.PosY + 10, 
                MultiImage[this.flowSN].SpriteWidth - 20, MultiImage[this.flowSN].SpriteHeight - 15);
            this.mouthBB = new CBoundingBox(this.PosX+130, this.PosY + 40, 20, 25);
        }
    };
    this.setState = function(st){
        //
        this.counter = 0;
        switch(st){
            case DOLPHIN_EAT:
                if(this.toLeft)
                    this.currentFrame = 6;// спрайты выводятся по два раза (всего - 7)
                else
                    this.currentFrame = 0;
                break;
            case DOLPHIN_DIE: 
                this.currentFrame = 0;
                if (this.trashId >= 0) {
                    Game.trashes[this.trashId].isVisible = false;
                    Game.trashes[this.trashId].isCaptured = true;                    
                }
                break;
            case DOLPHIN_DROPS: 
                this.currentFrame = MultiImage['dolphin_die_l'].SpriteCount - 1;
                break;
            case DOLPHIN_OUT:                
                this.state = st;
                Game.isLost();
                break;
        };        
        //      
        this.state = st;
    };
    this.updateBB = function(){
        //
        this.mouthBB.PosY = this.PosY + 40;
        this.bodyBB.PosY = this.PosY + 10;
        if(this.toLeft){
            this.mouthBB.PosX = this.PosX;
            this.bodyBB.PosX = this.PosX;            
        }    
        else{
            this.mouthBB.PosX = this.PosX + 130;
            this.bodyBB.PosX = this.PosX + 20;
        }
    };
    this.update = function(){
        //
        this.counter++;
        switch(this.state){
            case DOLPHIN_FLOW : 
                //
                if(this.counter === DOLPHIN_FLOW_UPDATES){
                    this.counter = 0;
                    if(!this.toLeft){
                        if(++this.currentFrame === MultiImage[this.flowSN].SpriteCount)
                            this.currentFrame = 0;
                    }
                    else
                        if(--this.currentFrame === -1)
                            this.currentFrame = MultiImage[this.flowSN].SpriteCount - 1;
                }
                this.PosX += this.shiftX;
                //
                if(this.toLeft && this.PosX <= -150){
                    this.PosX = 1000 + 500;
                    this.PosY = Math.round(Math.random() * 1000) % 480 + 200;
                }
                if(!this.toLeft && this.PosX >= 1000){
                    this.PosX = -150 - 500;
                    this.PosY = Math.round(Math.random() * 1000) % 480 + 200;
                }
                this.updateBB();
                //
                break;
            case DOLPHIN_EAT : 
                if (this.counter === DOLPHIN_EAT_UPDATES){
                    this.counter = 0;
                    if(this.toLeft){
                        this.currentFrame--;
                        if(this.currentFrame === -1)
                            this.setState(DOLPHIN_DIE);
                    }
                    else{
                        this.currentFrame++;
                        if(this.currentFrame === 7)
                            this.setState(DOLPHIN_DIE);
                    }                        
                }
                break;
            case DOLPHIN_DIE : 
                this.PosY += this.shiftY;
                if(this.PosY === 700){
                    this.setState(DOLPHIN_OUT);
                    break;
                }
                this.PosX += Math.round(Math.sin(this.PosY * 0.05)*1.5);                
                //
                if(this.counter === DOLPHIN_DIE_UPDATES){
                    this.counter = 0;
                    this.currentFrame++;
                    if(this.currentFrame === MultiImage[this.dieSN].SpriteCount)
                            this.setState(DOLPHIN_DROPS);                              
                }
                break;
            case DOLPHIN_DROPS : 
                this.PosY += this.shiftY;
                if(this.PosY >= 700){
                    this.setState(DOLPHIN_OUT);
                    break;
                }
                this.PosX += Math.round(Math.sin(this.PosY * 0.05)*1.5);
                break;
            case DOLPHIN_OUT : break;
        };
    };
    this.draw = function(context){
        //
        switch(this.state){
            case DOLPHIN_FLOW :
                context.drawImage(
                    MultiImage[this.flowSN],
                    this.currentFrame * MultiImage[this.flowSN].SpriteWidth, 
                    0,
                    MultiImage[this.flowSN].SpriteWidth,
                    MultiImage[this.flowSN].SpriteHeight,
                    this.PosX,
                    this.PosY,
                    MultiImage[this.flowSN].SpriteWidth,
                    MultiImage[this.flowSN].SpriteHeight);
                //
                break;
            case DOLPHIN_EAT :
                let sPos = 0;
                switch(this.currentFrame){
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        if(this.toLeft)
                            sPos = (3 - this.currentFrame) * MultiImage[this.eatSN].SpriteWidth;
                        else
                            sPos = this.currentFrame * MultiImage[this.eatSN].SpriteWidth;
                        break;
                    case 4:
                    case 5:
                    case 6:
                        if(this.toLeft)
                            sPos = (this.currentFrame - 3) * MultiImage[this.eatSN].SpriteWidth;
                        else
                            sPos = (6 - this.currentFrame) * MultiImage[this.eatSN].SpriteWidth;
                        break;
                };
                //
                context.drawImage(
                    MultiImage[this.eatSN],
                    sPos, 
                    0,
                    MultiImage[this.eatSN].SpriteWidth,
                    MultiImage[this.eatSN].SpriteHeight,
                    this.PosX,
                    this.PosY,
                    MultiImage[this.eatSN].SpriteWidth,
                    MultiImage[this.eatSN].SpriteHeight);                
                break;
            case DOLPHIN_DIE : 
                context.drawImage(
                    MultiImage[this.dieSN],
                    0, 
                    this.currentFrame * MultiImage[this.dieSN].SpriteHeight, 
                    MultiImage[this.dieSN].SpriteWidth,
                    MultiImage[this.dieSN].SpriteHeight,
                    this.PosX,
                    this.PosY,
                    MultiImage[this.dieSN].SpriteWidth,
                    MultiImage[this.dieSN].SpriteHeight);
                //    
                break;
            case DOLPHIN_DROPS : 
                context.drawImage(
                    MultiImage[this.dieSN],
                    0, 
                    (MultiImage[this.dieSN].SpriteCount - 1) * MultiImage[this.dieSN].SpriteHeight, 
                    MultiImage[this.dieSN].SpriteWidth,
                    MultiImage[this.dieSN].SpriteHeight,
                    this.PosX,
                    this.PosY,
                    MultiImage[this.dieSN].SpriteWidth,
                    MultiImage[this.dieSN].SpriteHeight);
                //    
                break;
            case DOLPHIN_OUT: break;            
        };
    };
};

