import React, { useEffect, useState, useRef } from "react";
import { useLocation } from 'react-router-dom';
import WhiteboardComponent from "./WhiteboardComponent";


function WhiteBoard() {
   

    const wBoard = useRef();

    const location = useLocation();
    const [chatroomId, setChatroomId] = useState("");
    const [token, setToken] = useState("");
    const [color, setColor] = useState();


    const Load = () => {
        setChatroomId(location.state.chatroomId);
        setToken(location.state.token);
    }


    //Run once after the initial render
    useEffect(() => {
        Load();
        
    }, []);

    useEffect(() => {
        wBoard.current.setColor(color);
    });

    return (

        <div className='whiteboard'>
            <div className='options'>

                <div className='colors'>

                    <button className='red' onClick={(e) => {
                        setColor(4294901760);
                    }}></button>

                    <button className='blue' onClick={(e) => {
                        setColor(4278190335);
                    }}></button>

                    <button className='green' onClick={(e) => {
                        setColor(4278255360);
                    }}></button>

                    <button className='yellow' onClick={(e) => {
                        setColor(4294967040);
                    }}></button>

                    <button className='black' onClick={(e) => {
                        setColor(4278190080);
                    }}></button>

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
                    <button className='clear' onClick={ (e) => {
                        setColor(4294967295);
                    }}>
                        <img src="/images/clear.png" ></img>
                    </button>

                </div>


            </div>
            <WhiteboardComponent id="whiteboardComponent" ref={wBoard} uri="wss://localhost:7043/ws/whiteboard" token={location.state.token} chatroomId={location.state.chatroomId} />
        </div>
    );

}
export default WhiteBoard;