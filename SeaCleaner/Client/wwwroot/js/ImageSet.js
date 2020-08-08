/* 
 * Объект - синглтон, хранящий картинки и анимации, 
 * которые используют несколько объектов одновременно 
 */
let MultiImage = {
    //
    trash_1 : new Image(),
    trash_2 : new Image(),
    trash_3 : new Image(),
    trash_4 : new Image(),
    trash_5 : new Image(),
    //
    fish_1_l : new Image(),
    fish_1_r : new Image(),
    fish_2_l : new Image(),
    fish_2_r : new Image(),
    fish_3_l : new Image(),
    fish_3_r : new Image(),
    //
    bubble : new Image(),
    //
    dolphin_flow_l : new Image(),
    dolphin_flow_r : new Image(),
    dolphin_eat_l : new Image(),
    dolphin_eat_r : new Image(),
    dolphin_die_l : new Image(),
    dolphin_die_r : new Image(),
    //
    loadImages : function(){
        //
        let OnLoad = function(){
            if(this.IsVertical){
                this.SpriteWidth = this.width;
                this.SpriteHeight = this.height / this.SpriteCount;
            }
            else{
                this.SpriteWidth = this.width / this.SpriteCount;
                this.SpriteHeight = this.height;
            }            
        };
        // статические изображения мусора
        this.trash_1.src = "/images/trash_1.png";
        this.trash_2.src = "/images/trash_2.png";
        this.trash_3.src = "/images/trash_3.png";
        this.trash_4.src = "/images/trash_4.png";
        this.trash_5.src = "/images/trash_5.png";
        ///////////////////////////////////////////////////////////////////////
        // анимированные стаи рыбок. По 7 тайлов горизонтально
        this.fish_1_l.IsVertical = false;
        this.fish_1_l.SpriteCount = 7;
        this.fish_1_l.onload = OnLoad;
        this.fish_1_l.src = "/images/fish_1_left.png";
        //
        this.fish_1_r.IsVertical = false;
        this.fish_1_r.SpriteCount = 7;
        this.fish_1_r.onload = OnLoad;
        this.fish_1_r.src = "/images/fish_1_right.png";
        //
        this.fish_2_l.IsVertical = false;
        this.fish_2_l.SpriteCount = 7;
        this.fish_2_l.onload = OnLoad;
        this.fish_2_l.src = "/images/fish_2_left.png";
        //
        this.fish_2_r.IsVertical = false;
        this.fish_2_r.SpriteCount = 7;
        this.fish_2_r.onload = OnLoad;
        this.fish_2_r.src = "/images/fish_2_right.png";
        //
        this.fish_3_l.IsVertical = false;
        this.fish_3_l.SpriteCount = 7;
        this.fish_3_l.onload = OnLoad;
        this.fish_3_l.src = "/images/fish_3_left.png";
        //
        this.fish_3_r.IsVertical = false;
        this.fish_3_r.SpriteCount = 7;
        this.fish_3_r.onload = OnLoad;
        this.fish_3_r.src = "/images/fish_3_right.png";
        ///////////////////////////////////////////////////////////////////////
        this.bubble.IsVertical = true;
        this.bubble.SpriteCount = 4;
        this.bubble.onload = OnLoad;
        this.bubble.src = "/images/bubble.png";
        //////////////////////////////////////////////////////////////////////
        this.dolphin_flow_l.IsVertical = false;
        this.dolphin_flow_l.SpriteCount = 5;
        this.dolphin_flow_l.onload = OnLoad;
        this.dolphin_flow_l.src = "/images/dolphin_flow_left.png";
        //
        this.dolphin_flow_r.IsVertical = false;
        this.dolphin_flow_r.SpriteCount = 5;
        this.dolphin_flow_r.onload = OnLoad;
        this.dolphin_flow_r.src = "/images/dolphin_flow_right.png";
        //
        this.dolphin_eat_l.IsVertical = false;
        this.dolphin_eat_l.SpriteCount = 4;
        this.dolphin_eat_l.onload = OnLoad;
        this.dolphin_eat_l.src = "/images/dolphin_eat_left.png";
        //
        this.dolphin_eat_r.IsVertical = false;
        this.dolphin_eat_r.SpriteCount = 4;
        this.dolphin_eat_r.onload = OnLoad;
        this.dolphin_eat_r.src = "/images/dolphin_eat_right.png";
        //
        this.dolphin_die_l.IsVertical = true;
        this.dolphin_die_l.SpriteCount = 9;
        this.dolphin_die_l.onload = OnLoad;
        this.dolphin_die_l.src = "/images/dolphin_die_left.png";
        //
        this.dolphin_die_r.IsVertical = true;
        this.dolphin_die_r.SpriteCount = 9;
        this.dolphin_die_r.onload = OnLoad;
        this.dolphin_die_r.src = "/images/dolphin_die_right.png";
    }
};

