import React, { useEffect, useState } from "react";
import { useLocation } from 'react-router-dom';


function WhiteBoard() {
    const script = document.createElement("script");
    script.src = "js/whiteboard.js";
    script.async = false;
    document.body.appendChild(script);

  

    const location = useLocation();
    const [chatroomId, setChatroomId] = useState("");
    const [token, setToken] = useState("");


    const Load = () => {
        //setChatroomId(location.state.chatroomId);
        //setToken(location.state.token);
    }


    //Run once after the initial render
    useEffect(() => {
        Load();
        const scriptLoad = document.createElement("script");
        scriptLoad.innerHTML =
            `setTimeout(function () {
                let c = document.getElementById("whiteboardCanvas")
                startIfNotStarted(c, "wss://localhost:7043/ws/whiteboard", 'asd', 10)
            }, 100);`
        scriptLoad.async = false;
        document.body.appendChild(scriptLoad);

    }, []);


    return (

        <div className='whiteboard'>
            <div className='options'>

                <div className='colors'>

                    <button className='red'>

                    </button>

                    <button className='blue'>

                    </button>

                    <button className='green'>

                    </button>

                    <button className='yellow'>

                    </button>

                    <button className='black'>

                    </button>

                </div>

                <div className='shapes'>
                    <button className='point'>
                        <img src="/images/point.png" ></img>
                    </button>

                    <button className='line'>
                        <img src="/images/line.png" ></img>
                    </button>

                    <button className='circle'
                        hidden
                    >
                        <img src="/images/circle.png" ></img>
                    </button>


                </div>

                <div className='actions'>
                    <button className='clear'>
                        <img src="/images/clear.png" ></img>
                    </button>

                </div>


            </div>

            <div className='canvas'>
                <canvas className='wCanvas' id="whiteboardCanvas" width="1000" height="800"></canvas>
            </div>




        </div>
    );

}
export default WhiteBoard;