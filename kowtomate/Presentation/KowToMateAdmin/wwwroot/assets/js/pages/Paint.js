
    var canvas, ctx, flag = false,
    prevX = 0,
    currX = 0,
    prevY = 0,
    currY = 0,
    dot_flag = false;

    var x = "red",
    y = 10;

    var rect;
    var img;
    var drivePath;
    var textBoxCount = 0;
    var stack = [];

    var drawActionConstants = {
        strokeDraw: "Stroke",
        textDraw: "Text"
    };

    var lastAction = null;

function init(imagePath, relativePath) {

    debugger;

        lastAction = null;
        stack = [];
        textBoxCount = 0;
        drivePath = relativePath;
        img = new Image();
        img.src = imagePath;

        img.onload = function () {
            canvas.width = img.width;
            canvas.height = img.height;
            ctx.drawImage(img, 0, 0);

            pushCanvasStateInStack();
        };

        //canvas.width = img.width;
        //canvas.height = img.height;
    
        w = canvas.width;
        h = canvas.height;

        AddEventsForMarkup();
    }

    function AddEventsForMarkup() {
        canvas.addEventListener("mousemove", mousemove);
        canvas.addEventListener("mousedown", mousedown);
        canvas.addEventListener("mouseup", mouseup);
        canvas.addEventListener("mouseout", mouseout);
    }

    function RemoveEventsForMarkup() {
        canvas.removeEventListener("mousemove", mousemove);
        canvas.removeEventListener("mousedown", mousedown);
        canvas.removeEventListener("mouseup", mouseup);
        canvas.removeEventListener("mouseout", mouseout);
    }

    function AddEventsForText() {
    debugger;
        canvas.addEventListener("mousedown", mousedownForText);
    }

    function RemoveEventsForText() {
        canvas.removeEventListener("mousedown", mousedownForText);
    }
    
    function mousemove(e) {
        findxy('move', e)
    }

    function mousedown(e) {
        findxy('down', e)
    }

    function mouseup(e) {
        findxy('up', e)
    }

    function mouseout(e) {
        findxy('out', e)
    }

    function mousedownForText(e) {
        var scrollTop = document.getElementById("preview-modal").scrollTop;
        
        addInput(e.clientX, e.clientY + scrollTop);
    }

    function color(obj) {
        switch (obj.id) {
            case "green":
                x = "green";
                break;
            case "blue":
                x = "blue";
                break;
            case "red":
                x = "red";
                break;
            case "yellow":
                x = "yellow";
                break;
            case "orange":
                x = "orange";
                break;
            case "black":
                x = "black";
                break;
            case "white":
                x = "white";
                break;
        }
        if (x == "white") y = 20;
        else y = 10;
    
    }

    function draw() {
        ctx.beginPath();
        ctx.moveTo(prevX, prevY);
        ctx.lineTo(currX, currY);
        ctx.strokeStyle = x;
        ctx.lineWidth = y;
        ctx.stroke();
        ctx.closePath();
    }

    function erase() {
        var m = confirm("Want to clear");
        if (m) {
            ctx.clearRect(0, 0, w, h);
            document.getElementById("canvasimg").style.display = "none";
        }
    }

    function clearCanvas() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, 0, 0);

        var textBoxes = document.getElementsByClassName("markup-text");

        while (textBoxes[0]) {
            textBoxes[0].parentNode.removeChild(textBoxes[0]);
        }


        var textBoxButtons = document.getElementsByClassName("markup-text-button");

        while (textBoxButtons[0]) {
            textBoxButtons[0].parentNode.removeChild(textBoxButtons[0]);
        }

        stack = [];
        lastAction = null;
        pushCanvasStateInStack();
    }

    function fullScreen() {
        if (canvas.webkitRequestFullScreen) {
            canvas.webkitRequestFullScreen();
        }
        else {
            canvas.mozRequestFullScreen();
        }
    }

function writeTextOnCanvas() {
    debugger;
        RemoveEventsForMarkup();

        AddEventsForText();
    }

    function paintOnCanvas() {
        RemoveEventsForText();

        AddEventsForMarkup();
    }

    function findxy(res, e) {
        rect = canvas.getBoundingClientRect();

        if (res == 'down') {
        
            prevX = currX;
            prevY = currY;

            currX = e.clientX - rect.left;
            currY = e.clientY - rect.top;

            //--- change ratio -----

            currX = (currX / canvas.clientWidth) * canvas.width;
            currY = (currY / canvas.clientHeight) * canvas.height;

            //-----------------------

            flag = true;
            dot_flag = true;
            if (dot_flag) {
                ctx.beginPath();
                ctx.fillStyle = x;
                ctx.fillRect(currX, currY, y, y);
                ctx.closePath();
                dot_flag = false;

                lastAction = drawActionConstants.strokeDraw;
            }
        }
        if (res == 'up' || res == "out") {

            if (flag) pushCanvasStateInStack();

            flag = false;

        }
        if (res == 'move') {
            if (flag) {
                prevX = currX;
                prevY = currY;
                currX = e.clientX - rect.left;
                currY = e.clientY - rect.top;

                currX = (currX / canvas.clientWidth) * canvas.width;
                currY = (currY / canvas.clientHeight) * canvas.height;

                draw();
                lastAction = drawActionConstants.strokeDraw;
            }
        }
    }

    function addInput(x, y) {
        textBoxCount++;

        var input = document.createElement('input');
        var button = document.createElement('button');


        button.style.position = 'absolute';
        button.style.fontSize = '20px';
        button.style.left = (x - 4) + 200 + 'px';
        button.style.top = (y - 4) + 'px';
        button.style.zIndex = 1;
        button.innerHTML = "&times";
        button.className = "markup-text-button";
        button.id = "markup-text-button-" + textBoxCount;

        button.addEventListener("click", handleClick);


        input.type = 'text';
        // Set font size for the input element
        input.style.fontSize = '20px';
        input.style.position = 'absolute';
        input.style.left = (x - 4) + 'px';
        input.style.top = (y - 4) + 'px';
        input.style.zIndex = 1;
        input.className = "markup-text";
        input.style.width = 200 + 'px';
        input.onkeydown = handleEnter;
        input.id = "markup-text-" + textBoxCount;

        document.getElementById("preview-modal").appendChild(input);
        document.getElementById("preview-modal").appendChild(button);

        input.focus();
    }

    function handleEnter(e) {
    
        var keyCode = e.keyCode;
        if (keyCode === 13) {

            let inputX, inputY;
            inputX = parseInt(this.style.left, 10);
            inputY = parseInt(this.style.top, 10);

            var scrollTop = document.getElementById("preview-modal").scrollTop;
            inputY -= scrollTop;

            drawText(this.value, inputX, inputY);

            
            var idCounts = this.id.split('-');
            var id = idCounts[idCounts.length - 1];
            var input = document.getElementById("markup-text-button-" + id);


            document.getElementById("preview-modal").removeChild(this);
            document.getElementById("preview-modal").removeChild(input);
        }
    }

    function handleClick(e) {
        
        var button = this;
        var id = button.id;
        var input = document.getElementById(id.replace("button-", ""));

        document.getElementById("preview-modal").removeChild(button);
        document.getElementById("preview-modal").removeChild(input);
    }

    function colorForText() {
        switch (x) {
            case "green":
                ctx.fillStyle = 'green';
                break;
            case "blue":
                ctx.fillStyle = 'blue';
                break;
            case "red":
                ctx.fillStyle = 'red';
                break;
            case "yellow":
                ctx.fillStyle = 'yellow';
                break;
            case "orange":
                ctx.fillStyle = 'orange';
                break;
            case "black":
                ctx.fillStyle = 'black';
                break;
            case "white":
                ctx.fillStyle = 'white';
                break;
        }

    }

    function drawText(txt, x, y) {
        rect = canvas.getBoundingClientRect();

        currX = x - rect.left;
        currY = y - rect.top;

        //--- change ratio -----

        currX = (currX / canvas.clientWidth) * canvas.width;
        currY = (currY / canvas.clientHeight) * canvas.height;

        //-----------------------


        ctx.textBaseline = 'top';
        ctx.textAlign = 'left';
        colorForText();
        ctx.font = '120px sans-serif';
        ctx.fillText(txt, currX + 4, currY + 4);

        lastAction = drawActionConstants.textDraw;

        pushCanvasStateInStack();
    }
    
    function undo() {
        debugger
        if (lastAction != null) {
            stack.pop();
        }

        var canvasState = (stack.length == 1 ? stack[0] : stack.pop());
        console.log(canvasState);

        let previousImg = new Image();
        previousImg.src = canvasState;

        previousImg.onload = function () {
            canvas.width = previousImg.width;
            canvas.height = previousImg.height;

            ctx.drawImage(previousImg, 0, 0);

            lastAction = null;
        };
    }

    function pushCanvasStateInStack() {
        stack.push(canvas.toDataURL("image/png"));
    }
        