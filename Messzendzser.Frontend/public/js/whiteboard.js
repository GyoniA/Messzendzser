
try {
    
} catch (e) {
    console.log("websocket error: "+e)
}
c = document.getElementById("whiteboardCanvas")
WhiteboardManager(c, "wss://localhost:7043/ws","token",10)


class WhiteboardManager{

    state = "new"

    constructor(canvas, websocketURI, token,chatroomId) {
        this.token = token
        this.canvas = canvas
        this.chatroomId = chatroomId
        this.socket = new WebSocket(websocketURI);
        socket.onopen = function (event) {
            console.log("Whiteboard connection established")
        };
        socket.onclose = function (event) {
            console.log("Whiteboard connection disconnected")
        };
        socket.onmessage = function (event) {
            console.log("Message received: "+event.Data)
            handleMessage(JSON.parse(event.Data))
        };
    }


    handleMessage(data) {
        if (state == "new") {
            data.Type == Ok
        } else if (state == "authenticated") {

        }
    }
}