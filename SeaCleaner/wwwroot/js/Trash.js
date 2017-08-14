/* 
 * класс для обработки столкновений
 */
function CBoundingBox(_X, _Y, _W, _H){
    this.PosX = _X;
    this.PosY = _Y;
    this.width = _W;
    this.height = _H;
    this.isPointInside = function(_X, _Y){
        if(((_X >= this.PosX) && (_X <= this.PosX + this.width)) &&
           ((_Y >= this.PosY) && (_Y <= this.PosY + this.height)))
            return true;
        else
            return false;
    };
    this.isIntersect = function(Bbox){
        // пересечение двух прямоугольных областей
        if(this.isPointInside(Bbox.PosX, Bbox.PosY) ||
           this.isPointInside(Bbox.PosX + Bbox.width, Bbox.PosY) ||
           this.isPointInside(Bbox.PosX, Bbox.PosY + Bbox.height)||
           this.isPointInside(Bbox.PosX + Bbox.width, Bbox.PosY + Bbox.height))
            return true;
        else
            return false;   
    };
}
/*
 * класс для мусора
 */
function CTrash(_SpriteName){
    this.spriteName = _SpriteName;
    this.PosX = Math.round(Math.random()*1000) % 800 + 100;
    this.PosY = Math.round(Math.random()*1000) % 450 + 200;
    this.PosY_ = this.PosY;
    this.fluctCoef = 0.1;
    this.fluctuation = 0.5;
    this.isCaptured = false;
    this.isVisible = true;
    this.counter = 0;
    this.__defineSetter__("posX", function(value){
        this.PosX = value;
        this.bBox.PosX = value;
    });
    this.__defineSetter__("posY", function(value){
        this.PosY = value;
        this.bBox.PosY = value;
        this.PosY_ = value;
    });
    this.initialize = function(){
        this.fCounter = Math.round(Math.random() * 10000) % 1800;
        this.width = MultiImage[this.spriteName].width;
        this.height = MultiImage[this.spriteName].height;
        this.bBox = new CBoundingBox(this.PosX, this.PosY, this.width, this.height);
    };
    this.update = function(){
        if(this.isCaptured) return;
        //
        if(++this.counter >= 2){
        
            this.fCounter++;
            if(this.fCounter === 1800){
                this.fCounter = 0;
                this.PosY = this.PosY_;
            }
            this.counter = 0;
        }
        if(!this.isCaptured){
            this.PosY += Math.sin(this.fCounter * this.fluctCoef) * this.fluctuation;
            this.bBox.PosY = this.PosY;
        }
    };
    this.draw = function(context){
        if(this.isVisible){
            context.drawImage( 
                MultiImage[this.spriteName], 
                this.PosX, this.PosY,  
                this.width, this.height);              
        }    
    };
}

