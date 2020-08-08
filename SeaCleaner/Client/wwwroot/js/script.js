window.onload = function () {
    document.getElementById('lblDolphinsCount').innerText = 4;
    document.getElementById('lblTrashesCount').innerText = 10;
    document.getElementById('lblGameTime').innerText = '0 : 0';
    setTimeout(function () {
        MultiImage.loadImages();
        Game.loadContent();
        document.getElementById('lblLoading').style.display = 'none';
        document.getElementById('container').style.display = 'block';
        setTimeout(function () {
            Game.initialize(document.getElementById('Screen'));
            Game.onDolphinDie = function (game) {
                document.getElementById('lblDolphinsCount').innerText = Game.getLiveDolphinsCount();
            };
            Game.onTrashLoad = function (game) {
                document.getElementById('lblTrashesCount').innerText = Game.getTrashesInWater();
            };

        }, 500);
    }, 500);
};

function Start() {
    if (Game.timerId === -1)
        Game.start(function (res) {

            setTimeout(
                function () {
                    if (res.victory === true) {

                        let gamerId = new URL(window.location).searchParams.get("gamerid");
                        if (gamerId === null) gamerId = "4BC2B0CF-0AE6-44FD-A94C-3B34D77B0C35";

                        let xhr = new XMLHttpRequest();
                        xhr.open("POST", 'api/win', true);
                        xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                                window.location.href = '/';
                            }
                        };
                        xhr.onerror = function (info) { console.log(info); };
                        xhr.send('Victory=' + res.victory + '&SavedDolphins=' + res.dolphins + '&Seconds=' + res.seconds + '&gamerid='+gamerId);
                    }
                    else
                        window.location.href = '/';
                },
                2000);

        });
    let labelTimerStartValue = Date.now();
    let labelTimerId = setInterval(function () {
        let secondsLeft = Math.floor((Date.now() - labelTimerStartValue) / 1000);
        let minutesLeft = Math.trunc(secondsLeft / 60);
        document.getElementById('lblGameTime').textContent = minutesLeft.toString() + ' : ' + (secondsLeft - minutesLeft * 60).toString();
    }, 1000);
}