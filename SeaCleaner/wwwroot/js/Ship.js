/* 
 * объект кораблик - взаимодействует со стрелой, винтом и ковшом
 */

let Ship = {
    PosX : 20,
    PosY : 24,
    shiftX : 6,
    shiftY : 0,
    pic : new Image(),
    fCounter : 0,
    fluctCoef : 0.15,
    fluctuation : 3.0,
    counter : 0,
    //
    trashes : [],
    //
    loadContent : function(){
        this.pic.src = '/images/ship.png';
        //
        Screw.loadContent();
        Arrow.loadContent();
        Hug.loadContent();
    },
    initialize : function(){
        //
        for(let i=0; i<10; i++)
            this.trashes.push(-1);
        //      
        //Screw.initialize();
        //Arrow.initialize();
        //Hug.initialize();
    },
    update : function(){
        if(++this.counter > 1){
            this.counter = 0;
            if (++this.fCounter >= 1800)
                this.fCounter = 0;
        }        
        //
        let d = 0;
        switch(Screw.state){
            case SCREW_STOPPED : 
                //
                break;
            case SCREW_LEFTWARD :
                this.PosX -= this.shiftX;
                if(this.PosX < -300)
                    this.PosX = -300;
                else
                    d = -this.shiftX;
                break;
            case SCREW_RIGHTWARD :
                this.PosX += this.shiftX;
                if(this.PosX > 850)
                    this.PosX = 850;
                else
                    d = this.shiftX;
                break;
        }
        //
        this.shiftY = Math.sin(this.fCounter * this.fluctCoef) * this.fluctuation;
        //
        let i = 0;
        while(this.trashes[i] >=0){
            Game.trashes[this.trashes[i]].PosX += d;
            switch(i){
                case 0: case 1: case 2: case 3:
                    Game.trashes[this.trashes[i]].PosY = this.PosY + 80 + this.shiftY;
                    break;
                case 4: case 5: case 6:    
                    Game.trashes[this.trashes[i]].PosY = this.PosY + 68 + this.shiftY;
                    break;       
                case 7: case 8:
                    Game.trashes[this.trashes[i]].PosY = this.PosY + 56 + this.shiftY;
                    break;
                case 9:
                    Game.trashes[this.trashes[i]].PosY = this.PosY + 44 + this.shiftY;
                    break;
            }            
            i++;
        }
        //
        Screw.PosX = this.PosX + 40;
        Screw.PosY = this.PosY + 135 + this.shiftY;
        Screw.update();
        //
        Arrow.PosX = this.PosX + 125;
        Arrow.PosY = this.PosY - 18  + this.shiftY;
        Arrow.update();
        //        
    },
    draw : function(context){   
        //
        
        //
        Arrow.draw(context);
        context.drawImage(
            this.pic,
            this.PosX,
            this.PosY + this.shiftY,
            this.pic.width,
            this.pic.height);        
        //
        Screw.draw(context); 
        Hug.draw(context);
    },
    addTrash : function(trashID){        
        let i = 0;
        while(this.trashes[i] >= 0) i++;
        this.trashes[i] = trashID;
        switch(i){
            case 0:
            case 1:
            case 2:
            case 3:
                Game.trashes[trashID].PosX = this.PosX + 100 + 23*i;
                Game.trashes[trashID].PosY = this.PosY + 80;
                break;
            case 4:
            case 5:
            case 6:
                Game.trashes[trashID].PosX = this.PosX + 111 + 23*(i%4);
                Game.trashes[trashID].PosY = this.PosY + 68;
                break;
            case 7:
            case 8:
                Game.trashes[trashID].PosX = this.PosX + 122 + 23*(i%7);
                Game.trashes[trashID].PosY = this.PosY + 56;
                break;
            case 9:
                Game.trashes[trashID].PosX = this.PosX + 130;
                Game.trashes[trashID].PosY = this.PosY + 44;
                break;
        }
        Game.trashes[trashID].isVisible = true;
        Game.trashes[trashID].isCaptured = true;
        Game.onTrashLoad(Game);
    }
};

