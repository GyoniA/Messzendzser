class WhiteboardEvent {
    constructor() {
        var self = this;
    }
}

class Point {
    constructor(X, Y) {
        this.X = X;
        this.Y = Y;
    }
}


class WhiteboardDotEvent extends WhiteboardEvent {
    constructor(X, Y, color) {
        super();
        this.Position = new Point(X, Y);
        this.Color = color
    }
}

class WhiteboardLineEvent extends WhiteboardEvent {
    constructor(startX, startY, endX, endY, color) {
        super();
        this.Start = new Point(startX, startY);
        this.End = new Point(endX, endY);

        this.Position.Color = color
    }
}

class WhiteboardEventMessage {
    constructor(events) {
        this.Type = 4;
        this.Events = events;
    }
}


class WhiteboardManager {
    constructor(canvas, websocketURI, token, chatroomId) {
        var self = this;
        this.state = "new";
        this.socket = new WebSocket(websocketURI);
        this.canvas = canvas;
        self.mousedown = false;
        self.events = [];

        this.sendMessage = function (json) {
            console.log('Sending data: ' + json);
            self.socket.send(json)
        }

        this.handleMessage = function (data) {
            if (self.state == "new") {
                if (data.Type == 2) {
                    console.log("authentication succcessful")
                    self.state = "authenticated"
                    setInterval(self.sendEvents, 500, null);
                }
            } else if (this.state == "authenticated") {
                if (data.Type == 3) {
                    var auth = { "Type": 2 };
                    var json = JSON.stringify(auth);
                    self.sendMessage(json);
                }
            }
        }


        this.sendAuthMessage = function () {
            var auth = { "Type": 0, "Username": "Hello", "Password": "World", "ChatroomId": chatroomId };
            var json = JSON.stringify(auth);
            self.sendMessage(json);
        }
        this.canvas.addEventListener("mousedown", function (e) { self.mousedown = true });
        this.canvas.addEventListener("mouseup", function (e) { self.mousedown = false });
        this.canvas.addEventListener("mousemove", function (e) {
            if (self.mousedown) {
                if (!e) e = window.event;

                var ctx = canvas.getContext("2d");
                var x = e.offsetX == undefined ? e.layerX : e.offsetX;
                var y = e.offsetY == undefined ? e.layerY : e.offsetY;
                ctx.fillRect(x, y, 3, 3);
                self.events.push(new WhiteboardDotEvent(x,y,0xFF000000));
            }
        });

        this.sendEvents = function () {
            if (self.events.length > 0) {
                console.log('sending events');
                var eventsMessage = new WhiteboardEventMessage(self.events);
                self.events = [];
                var json = JSON.stringify(eventsMessage);
                self.sendMessage(json);
            }

        }
        this.token = token
        this.canvas = canvas
        this.chatroomId = chatroomId
        
        this.socket.onopen = function (event) {
            console.log("Whiteboard connection established")
            
            self.sendAuthMessage()
        };
        this.socket.onclose = function (event) {
            console.log("Whiteboard connection disconnected")
        };
        this.socket.onmessage = function (event) {
            console.log("Message received: " + event.data)
            self.handleMessage(JSON.parse(event.data))
        };
    }


    
}

window.onload = function () {
    let c = document.getElementById("whiteboardCanvas")
    let wbMan = new WhiteboardManager(c, "wss://localhost:7043/ws/whiteboard", "token", 10)
};


