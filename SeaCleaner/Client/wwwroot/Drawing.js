let imagesLib = {};
let _2dcontext;

async function loadImage(pathToImage, spriteName) {
    let prom = new Promise((resolve, reject) => {
        let img = new Image();
        img.onload = () => resolve([img.width, img.height, img]);
        img.onerror = reject;
        img.src = pathToImage;
    });

    let imgData = await prom;
    imagesLib[spriteName] = imgData[2];

    return [imgData[0], imgData[1]];
}

function initializeScreen(elementId) {
    let element = document.getElementById(elementId);
    _2dcontext = element.getContext('2d');
}

function drawImage(spriteName, posX, posY, width, height) {

    if (_2dcontext === null) return;

    _2dcontext.drawImage(
        imagesLib[spriteName],
        posX, posY,
        width, height);
}

function drawSprite(spriteName, spriteX, spriteY, spriteWidth, spriteHeight, posX, posY) {

    if (_2dcontext === null) return;

    _2dcontext.drawImage(
        imagesLib[spriteName],
        spriteX,
        spriteY,
        spriteWidth,
        spriteHeight,
        posX,
        posY,
        spriteWidth,
        spriteHeight
    );
}

function clearResources() {
    imagesLib = {};
    _2dcontext = null;
}

function drawRect(x, y, w, h) {

    if (_2dcontext === null) return;

    _2dcontext.beginPath();
    _2dcontext.rect(x, y, w, h);
    _2dcontext.closePath();
    _2dcontext.strokeStyle = "red";
    _2dcontext.stroke();
}

let _gameDotNetObj;

let keyDownFunc = function (event) {

    if (_gameDotNetObj === null) return;

    _gameDotNetObj.invokeMethodAsync('OnKeyDown', event.keyCode);
}

let keyUpFunc = function (event) {

    if (_gameDotNetObj === null) return;

    _gameDotNetObj.invokeMethodAsync('OnKeyUp', event.keyCode);
}

function setupKeyEvents(gameDotNetObj) {
    _gameDotNetObj = gameDotNetObj;

    window.addEventListener('keydown', keyDownFunc, true);
    window.addEventListener('keyup', keyUpFunc, true);
}

function clearKeyEvents() {
    window.removeEventListener('keydown', keyDownFunc);
    window.removeEventListener('keyup', keyUpFunc);
    _gameDotNetObj = null;
}

function drawLine(startX, startY, endX, endY) {

    if (_2dcontext === null) return;

    _2dcontext.beginPath();
    _2dcontext.moveTo(startX, startY);
    _2dcontext.lineTo(endX, endY);
    _2dcontext.strokeStyle = "black";
    _2dcontext.stroke();
    _2dcontext.closePath();
}