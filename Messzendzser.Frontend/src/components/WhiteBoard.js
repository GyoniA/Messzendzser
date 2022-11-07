import React, { useEffect, useState } from "react";
import { useLocation } from 'react-router-dom';
import WhiteboardComponent from "./WhiteboardComponent";


function WhiteBoard() {
   

  

    const location = useLocation();
    const [chatroomId, setChatroomId] = useState("");
    const [token, setToken] = useState("");


    const Load = () => {
        setChatroomId(location.state.chatroomId);
        setToken(location.state.token);
    }


    //Run once after the initial render
    useEffect(() => {
        Load();

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

                    <p>{chatroomId}</p>
                    <p>{token}</p>

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
            <WhiteboardComponent uri="wss://localhost:7043/ws/whiteboard" token={location.state.token} chatroomId={location.state.chatroomId} />
        </div>
    );

}
export default WhiteBoard;