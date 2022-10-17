class WhiteboardEvent {
    constructor() {
        var self = this;
    }
}


class WhiteboardDotEvent extends WhiteboardEvent {
    constructor(X,Y,color) {
        this.Position.X = X 
        this.Position.Y = X 
        this.Position.Color = color
    }
}


class WhiteboardManager {
    constructor(canvas, websocketURI, token, chatroomId) {
        var self = this;
        this.state = "new";
        this.socket = new WebSocket(websocketURI);

        this.handleMessage = function (data) {
            if (self.state == "new") {
                if (data.Type == 2) {
                    console.log("authentication succcessful")
                    self.state = "authenticated"
                }
            } else if (this.state == "authenticated") {
                if (data.Type == 3) {
                    var auth = { "Type": 2 };
                    var json = JSON.stringify(auth);
                    self.socket.send(json)
                }
            }
        }


        this.sendAuthMessage = function () {
            var auth = { "Type": 0, "Username": "Hello", "Password": "World", "ChatroomId": chatroomId };
            var json = JSON.stringify(auth);
            self.socket.send(json);
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

c = document.getElementById("whiteboardCanvas")
let wbMan = new WhiteboardManager(c, "wss://localhost:7043/ws/whiteboard","token",10)


