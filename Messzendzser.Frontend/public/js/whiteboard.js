class WhiteboardManager {
    constructor(canvas, websocketURI, token, chatroomId) {
        var self = this;
        this.handleMessage = function (data) {
            if (state == "new") {
                if (data.Type == 2) {
                    console.log("authentication succcessful")
                    this.state = "auth"
                }
            } else if (this.state == "authenticated") {

            }
        }
        this.sendAuthMessage = function () {
            var auth = { "Type": 0, "Username": "Hello", "Password": "World", "ChatroomId": chatroomId };
            var json = JSON.stringify(auth);
            self.socket.send(json);
        }

        this.state = "new";

        this.token = token
        this.canvas = canvas
        this.chatroomId = chatroomId
        this.socket = new WebSocket(websocketURI);
        this.socket.onopen = function (event) {
            console.log("Whiteboard connection established")
            self.sendAuthMessage()
        };
        this.socket.onclose = function (event) {
            console.log("Whiteboard connection disconnected")
        };
        this.socket.onmessage = function (event) {
            console.log("Message received: " + event.Data)
            self.handleMessage(JSON.parse(event.Data))
        };
    }


    
}

c = document.getElementById("whiteboardCanvas")
let wbMan = new WhiteboardManager(c, "wss://localhost:7043/ws/whiteboard","token",10)


