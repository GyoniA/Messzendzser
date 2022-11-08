import React, { useEffect, useState } from "react";

class WhiteboardComponent extends React.Component {


    constructor(props) {
        super(props);
        this.props = props
        this.addedScript = false;
    }

    componentDidMount() {
        // Connect
        if (this.addedScript === false) {
            let container = document.getElementById("canvasContainer");
            const script = document.createElement("script");
            script.src = "js/whiteboard.js";
            script.async = false;
            container.appendChild(script);
            
            const scriptLoad = document.createElement("script");
            scriptLoad.innerHTML =
                `let manager = null;
                setTimeout(function () {
                    let c = document.getElementById("whiteboardCanvas")
                    manager = new WhiteboardManager(c, '`+ this.props.uri + `', '` + this.props.token + `', ` + this.props.chatroomId +`);
                }, 100);`
            scriptLoad.async = false;
            container.appendChild(scriptLoad);
            this.addedScript = true;
        }

    }

    setColor = (color) => {

        //console.log(color);

        

        let container = document.getElementById("canvasContainer");
        const scriptLoad = document.createElement("script");
        scriptLoad.innerHTML =
            `manager.setColor(`+ color+ `);`
        scriptLoad.async = false;
        container.appendChild(scriptLoad);

    }

    componentWillUnmount() {
        // Disconnect
    }

    render() {
        return (
        <div className='canvas' id='canvasContainer'>
            <canvas className='wCanvas' id="whiteboardCanvas" width="1000" height="800"></canvas>
        </div>
        );
    }
}

export default WhiteboardComponent;