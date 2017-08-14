/*
 * Класс для вывода анимированных волн  
 */
function CWaves(_Ypos, _Shift, _Width){
    this.PosX = 0; // положение по иксу будет пересчитываться в отрицательную сторону
    this.PosY = _Ypos; 
    this.Shift = _Shift; // смещение на определенное количество пикселей влево при каждой отрисовке 
    this.WavesWidth = _Width; // ширина волн на экране в пикселях
    //
    this.CurrentFrame = 0; // номер первого слева фрейма, от которого отсчитываются все остальные 
    this.UpdatesCounter = 0; // счетчик для подсчета вызовов Update()
    //
    this.LoadSprite = function(_Updates, _Frames, BackFileName, TopFileName){
        this.UpdatesOnFrame = _Updates; // количество вызовов Update() для перехода к новому фрейму
        this.FramesCount = _Frames; // количество фреймов
        // загрузка кртинок
        this.TopPic = new Image();
        this.TopPic.src = TopFileName;
        this.BkPic = new Image();
        this.BkPic.FramesCount = _Frames;
        this.BkPic.WavesWidth = this.WavesWidth;
        this.BkPic.onload = function(){
            // Ширина и высота фрейма
            this.FrameWidth = this.width / this.FramesCount;
            this.FrameHeight = this.height; 
            // количество фреймов на всю длину экрана
            this.FramesOnWidth = this.WavesWidth / this.FrameWidth + 1;
        };
        this.BkPic.src = BackFileName;        
    };
    this.Update = function(){
        // обновление счетчика
        if(++this.UpdatesCounter >= this.UpdatesOnFrame){
            this.PosX += this.Shift;
            if(this.PosX >= 0)
                this.PosX = -this.BkPic.FrameWidth + this.Shift;
            else{
                this.CurrentFrame++;
                if(this.CurrentFrame >= this.FramesCount)
                    this.CurrentFrame = 0;
            }           
            this.UpdatesCounter = 0;
        }
    };
    this._Draw = function(pic, context){
        var X = this.PosX, Y = this.PosY;
        var PicX = this.CurrentFrame * this.BkPic.FrameWidth, PicY = 0;
        for(var i = 0; i< this.BkPic.FramesOnWidth; i++){
            context.drawImage( 
            pic, // собственно, картинка
            PicX, PicY, // левая верхняя точка копируемой области
            this.BkPic.FrameWidth, this.BkPic.FrameHeight, //  и размеры
            X, Y,  // позиция вывода 
            this.BkPic.FrameWidth, this.BkPic.FrameHeight); //
            //
            X += this.BkPic.FrameWidth;
            PicX += this.BkPic.FrameWidth;
            if(PicX >= this.FramesCount * this.BkPic.FrameWidth)
                PicX = 0;
        }        
    };
    this.drawBk = function(context){
        this._Draw(this.BkPic, context);
    };
    this.drawTop = function(context){
        this._Draw(this.TopPic, context);
    };
}