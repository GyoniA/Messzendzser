import React, { useEffect, useState } from "react";


function WhiteBoard() {
    const script = document.createElement("script");
    script.src = "js/whiteboard.js";
    script.async = false;
    document.body.appendChild(script);

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

                    <button className='circle'>
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
            </div>




        </div>
    );
}
export default WhiteBoard;