import React from "react";

const decideCall= props => {
    return (
        <div className="decide-call">
            <div className="caller">
                <h3>{props.content}</h3>
            </div>
            <div className="buttons">
                <button className='pick-up'>
                    <img src = "/images/pickup.png" ></img>
                </button>
                <button className='decline' onClick={props.handleClose}>
                    <img src = "/images/dropcall.png" ></img>
                </button>
            </div>
        </div>
    );
};

export default decideCall;