
import React from "react";
import { useEffect, useState } from "react";

import PropTypes from "prop-types";

const DecideCall = (props) => {

    const [show, setShow] = useState(false);

    const closeHandler = (e) => {
        setShow(false);
        props.onClose(false);
    };

    const changeHandler = (e) => {
        setShow(false);
        props.onChange(false);
    };


    useEffect(() => {
        setShow(props.show);
    }, [props.show]);

    return (

        <div
            style={{
                visibility: show ? "visible" : "hidden",
                opacity: show ? "1" : "0"
            }}
            className="overlay">

            <div className="popup">
                <h2>Hivas:</h2>
                <h3>{props.name}</h3>
                
                <div className="button">

                <div className="buttons">
                        <button className='pick-up' onClick={changeHandler}>
                    <img src = "/images/pickup.png" ></img>
                </button>
                <button className='decline' onClick={closeHandler}>
                    <img src = "/images/dropcall.png" ></img>
                </button>
            </div>
                </div>
                
            </div>
            
        </div>
    );
};

DecideCall.propTypes = {
    name: PropTypes.string.isRequired,
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    onChange: PropTypes.func.isRequired
};

export default DecideCall;