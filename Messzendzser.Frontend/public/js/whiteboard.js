class WhiteboardManager {

    state = "new";
    sendAuthMessage = function() {
        var auth = { "Type": 0, "Username": "Hello", "Password": "World", "ChatroomId": chatroomId };
        var json = JSON.stringify(auth);
        this.socket.send(json);
    }


    handleMessage = function (data) {
        if (state == "new") {
            if (data.Type == 2) {
                console.log("authentication succcessful")
                this.state = "auth"
            }
        } else if (this.state == "authenticated") {

        }
    }
    constructor(canvas, websocketURI, token, chatroomId) {
        this.token = token
        this.canvas = canvas
        this.chatroomId = chatroomId
        this.socket = new WebSocket(websocketURI);
        this.socket.onopen = function (event) {
            console.log("Whiteboard connection established")
            this.sendAuthMessage()
        };
        this.socket.onclose = function (event) {
            console.log("Whiteboard connection disconnected")
        };
        this.socket.onmessage = function (event) {
            console.log("Message received: " + event.Data)
            this.handleMessage(JSON.parse(event.Data))
        };
    }


    
}

c = document.getElementById("whiteboardCanvas")
let wbMan = new WhiteboardManager(c, "wss://localhost:7043/ws/whiteboard","token",10)


