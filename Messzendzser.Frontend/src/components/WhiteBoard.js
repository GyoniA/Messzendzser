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
        setColor(4278190080);
    }


    //Run once after the initial render
    useEffect(() => {
        Load();

    }, []);

    useEffect(() => {
        if (color !== undefined) {
            wBoard.current.setColor(color);
        }
    });

    return (

        <div id='whiteboard'>
            <div id='options'>

                <div id='whiteboard_colors' >

                    <button id='red' onClick={(e) => {
                        setColor(4294901760);
                    }}></button>

                    <button id='blue' onClick={(e) => {
                        setColor(4278190335);
                    }}></button>

                    <button id='green' onClick={(e) => {
                        setColor(4278255360);
                    }}></button>

                    <button id='yellow' onClick={(e) => {
                        setColor(4294967040);
                    }}></button>

                    <button id='black' onClick={(e) => {
                        setColor(4278190080);
                    }}></button>

                </div>
                <button id='clear' onClick={(e) => {
                    wBoard.current.clear();
                }}>
                    <img src="/images/clear.png" ></img>
                </button>




            </div>
            <WhiteboardComponent id="whiteboardComponent" ref={wBoard} uri="wss://localhost:7043/ws/whiteboard" token={location.state.token} chatroomId={location.state.chatroomId} />
        </div>
    );

}
export default WhiteBoard;