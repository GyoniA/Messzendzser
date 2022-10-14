import React from "react";

const inCall= props => {
    return (
        <div className="in-call">
            <div className="caller">
                <h3>{props.content}</h3>
            </div>
            <div className="button">
                
                <button className='decline' onClick={props.handleClose}>
                    <img src = "/images/dropcall.png" ></img>
                </button>
            </div>
        </div>
    );
};

export default inCall;