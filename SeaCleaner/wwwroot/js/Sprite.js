//
// Класс для хранения неанимированного изображения 
// _X, _Y - координаты изображения
// _H, _W - высота и ширина картинки при ее отрисовке
function CSprite(_X, _Y, _W, _H){
    this.PosX = _X;
    this.PosY = _Y;
    this.Width = _W;
    this.Height = _H;
    // метод загрузки картинки   
    this.loadSprite = function(ImageFileName){
        this.pic = new Image();
        this.pic.src = ImageFileName;
    };
    // метод для вывода картинки на канву
    this.draw = function(context){
        context.drawImage( 
            this.pic, // собственно, картинка
            this.PosX, this.PosY,  // позиция вывода
            this.Width, this.Height); //  и размеры
        
    };    
};
/*
 *    класс-анимация использующая картинку из хранилища
 */
function CAnimation(_X, _Y, _Updates, _Shift_X, _Shift_Y, _SpriteName){
    this.PosX = _X;
    this.PosY = _Y;
    this.UpdatesOnFrame = _Updates;
    this.UpdatesShiftX = _Shift_X; // 
    this.UpdatesShiftY = _Shift_Y;
    this.SpriteName = _SpriteName;
    //
    this.UpdatesCounter = 0;
    this.CurrentFrame = 0;
    //
    this.update = function(){
        //
        this.PosX += this.UpdatesShiftX;
        this.PosY += this.UpdatesShiftY;
        //
        if(++this.UpdatesCounter >= this.UpdatesOnFrame){
            if(MultiImage[this.SpriteName].SpriteCount <= this.CurrentFrame + 1)
                this.CurrentFrame = 0;
            else
                this.CurrentFrame++;
            this.UpdatesCounter = 0;
        }
    };
    //
    this.draw = function(context){
        if(MultiImage[this.SpriteName].IsVertical)
            context.drawImage(
                MultiImage[this.SpriteName],
                0, 
                this.CurrentFrame * MultiImage[this.SpriteName].SpriteHeight,
                MultiImage[this.SpriteName].SpriteWidth,
                MultiImage[this.SpriteName].SpriteHeight,
                this.PosX,
                this.PosY,
                MultiImage[this.SpriteName].SpriteWidth,
                MultiImage[this.SpriteName].SpriteHeight         
            );
        else
            context.drawImage(
                MultiImage[this.SpriteName],
                this.CurrentFrame * MultiImage[this.SpriteName].SpriteWidth, 
                0,
                MultiImage[this.SpriteName].SpriteWidth,
                MultiImage[this.SpriteName].SpriteHeight,
                this.PosX,
                this.PosY,
                MultiImage[this.SpriteName].SpriteWidth,
                MultiImage[this.SpriteName].SpriteHeight         
            );                    
    };
};
/* 
 *  Класс Пузырь.
 */
function CBubble(_X, _Y, _Updates, _Shift_Y, _Fluctuation){
    CAnimation.apply(this, [_X, _Y, _Updates, 0, _Shift_Y, "bubble"]);
    this._Update = this.update;
    this.fluctuation = _Fluctuation; // повышает синусоиду 
    this.fluctCoeff = 0.1; // удлиняет синусоиду
    //
    this.update = function(){
        // движение пузыря по синусоиде
        this.PosX += Math.sin(this.PosY * this.fluctCoeff)*this.fluctuation;
        // автоматическое перемещение на дно в случае достижения волн
        if(this.PosY <= 150){
            this.PosY = 720 + Math.round(Math.random()*1000) % 30;
            this.PosX = Math.random() * SCREEN_WIDTH; // получается как раз 0 <= X <= 1000
        }                     
        //
        this._Update();        
    };
};
/*
 * Класс Стая рыбок
 */
function CFishes(_Updates, _Shift_X, _toLeft, _spriteName){
    CAnimation.apply(this, [300, 200, _Updates, _Shift_X, 0, _spriteName]);
    this.toLeft = _toLeft;
    //
    this.initialize = function(){
        this.UpdatesShiftX = Math.round(Math.random()*1000) % 4 + 3;
        this.PosY = Math.random() * 1000 % 480/*550*/ + 170; 
        if(this.toLeft){
            this.PosX = 1050 + Math.round(Math.random() * 1000 / 3);
            if(this.UpdatesShiftX > 0)
                this.UpdatesShiftX = -this.UpdatesShiftX;
        }
        else{
            this.PosX = -50 - Math.round(Math.random() * 1000 / 3);
            if(this.UpdatesShiftX < 0)
                this.UpdatesShiftX = -this.UpdatesShiftX;
        }       
    };
    this.isOffScreen = function(){
        if(this.toLeft)
            if(this.PosX < -50)
                return true;
            else
                return false;
        else
            if(this.PosX > 1050)
                return true;
            else
                return false;
    };
};
