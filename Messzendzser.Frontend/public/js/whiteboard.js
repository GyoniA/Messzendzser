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

class WhiteboardImageEvent extends WhiteboardEvent {
    constructor() {
        super();
        this.$type = "Messzendzser.WhiteBoard.WhiteboardImageEvent, Messzendzser";
        this.Type = 0;
    }
}

class WhiteboardDotEvent extends WhiteboardEvent {
    constructor(X, Y, color) {
        super();
        this.$type = "Messzendzser.WhiteBoard.WhiteboardDotEvent, Messzendzser";
        this.Type = 1;
        this.Position = new Point(X, Y);
        this.Color = color;
    }
}

class WhiteboardLineEvent extends WhiteboardEvent {
    constructor(startX, startY, endX, endY, color) {
        super();
        this.Start = new Point(startX, startY);
        this.End = new Point(endX, endY);

        this.$type = "Messzendzser.WhiteBoard.WhiteboardLineEvent, Messzendzser"
        this.Type = 2;
        this.Position.Color = color
    }
}

class WhiteboardEventMessage {
    constructor(events) {
        this.$type = "Messzendzser.WhiteBoard.WhiteboardEventMessage, Messzendzser"
        this.Type = 4;
        this.Events = events;
    }
}

class WhiteboardOkMessage {
    constructor() {
        this.$type = "Messzendzser.WhiteBoard.WhiteboardOKMessage, Messzendzser"
        this.Type = 2;
    }
}

var started = false;

function startIfNotStarted(canvas,uri,token,chatroomId) {
    if (!started) {
        started = true;
        let wbMan = new WhiteboardManager(canvas, uri, token, chatroomId);
    }
}

class WhiteboardManager {
    constructor(canvas, websocketURI, token, chatroomId) {
        var self = this;
        this.state = "new";
        this.socket = new WebSocket(websocketURI);
        this.canvas = canvas;
        self.canvasContext = canvas.getContext("2d");
        self.token = token;
        self.mousedown = false;
        self.events = [];

        this.color = 4294901760;//4278190080; // Black color with 255 Alpha

        this.setColor = function (newColor) {
            self.color = newColor;
        }

        this.sendMessage = function (json) {
            console.log('Sending data: ' + json);
            self.socket.send(json)
        }

        this.drawImage = function (b64) {
            var image = new Image();
            image.src = 'data:image/png;base64,' + b64;
            setTimeout(function () { self.canvasContext.drawImage(image, 0, 0) }, 100);
            
        }

        this.drawDot = function (x, y, color) {
            //self.canvasContext.fillStyle = color
            // todo set color
            var c = '#' + color.toString(16).substring(2);
            self.canvasContext.fillStyle = c;
            self.canvasContext.fillRect(x, y, 3, 3);
        }

        this.handleEvent = function (event) {
            if (event.Type == 1) { // Dot event
                self.drawDot(event.Position.X, event.Position.Y, event.Color);
            } else if (event.Type == 0) {
                self.drawImage(event.Image)
            }
        }


        this.handleMessage = function (data) {
            if (self.state == "new") {
                if (data.Type == 2) {
                    console.log("authentication succcessful")
                    self.state = "authenticated"
                    setInterval(self.sendEvents, 50, null);
                }
            } else if (this.state == "authenticated") {
                if (data.Type == 3) {
                    var alive = new WhiteboardOkMessage();
                    var json = JSON.stringify(alive);
                    self.sendMessage(json);
                } else if (data.Type == 4) { // Event
                    data.Events.forEach((event, index) => { self.handleEvent(event) });
                }
            }
        }


        this.sendAuthMessage = function () {
            var auth = { "$type": "Messzendzser.WhiteBoard.WhiteboardAuthenticationMessage, Messzendzser", "Type": 0, "Username": token, "Password": "World", "ChatroomId": chatroomId };
            var json = JSON.stringify(auth);
            self.sendMessage(json);
        }
        this.canvas.addEventListener("mousedown", function (e) { self.mousedown = true });
        this.canvas.addEventListener("mouseup", function (e) { self.mousedown = false });
        this.canvas.addEventListener("mousemove", function (e) {
            if (self.mousedown) {
                if (!e) e = window.event;

                var x = e.offsetX == undefined ? e.layerX : e.offsetX;
                var y = e.offsetY == undefined ? e.layerY : e.offsetY;
                self.drawDot(x, y, self.color);
                self.events.push(new WhiteboardDotEvent(x,y,self.color));
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
            console.log("Whiteboard connection established");
            self.drawImage("iVBORw0KGgoAAAANSUhEUgAAAEAAAAAwCAIAAAAuKetIAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAHlSURBVGhD7dNrbsMwDAPg3f+4u0Cn1kwm05ajh9uiQD/ozxJHNIft5/bhvgXe7VvA8vtjzlZb19FFPVO2qQBdKzoF5QJ0lcqk1AqoeKJfBSauUEAFW3CSqA/nE5Eq0Oet4ZOpfk83btUCl/DJgtrWjU+8gMpwwoc9vHvQO//HJ1igzyA4019O4OkBTwe0/D4OkQK0PVUAjwy0/z5XXloAPy9RBL60uQvQXhnfhURbIPDzEqfILGULHBBrw7kHPFq6nzOypnwFaKPMAbE2nHvAo6X7OcqSsaUK9JA8gxMKXhhwSCwTtQ0FGlzhgKczODHA64YSZQyOAr5FIbiygheaL/c9BVx8ud8Cz+PLzRfAH+/y/7XEyCXJArj7oT3cbJY72lOgaa+2meWOdhYQ7e0es9yRL3K2C1cetLdVlChjyBcQuPIMTgTh48OYOEoVkFGQZsChAV5fGeOIr4A4rz7biLQnmMZp2QIyPQTuNgYRdwGhb2/sRewmi6BToYCMAflxvF/mSqSAoO0yS7jXDE5otFnGIVhAUEabItp2jsOmAjJptOccn1QwJekJoW/1uGV/c5Q3zgKdHCciW6Ch4PrE1QoIukFlUsoFGrpKdAo2FWjoWp4p21pAo4vq2eppBV7lwwvcbn9O1Og8s1JeKwAAAABJRU5ErkJggg==");
            
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



