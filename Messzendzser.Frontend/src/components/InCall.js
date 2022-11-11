import React from "react";
import { useEffect, useState } from "react";

import PropTypes from "prop-types";

const InCall = (props) => {

    const [show, setShow] = useState(false);

    const closeHandler = (e) => {
        setShow(false);
        props.onClose(false);
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

                    <button className='decline' onClick={closeHandler}>
                        <img src="/images/dropcall.png" ></img>
                    </button>
                </div>
                
            </div>
            
        </div>
    );
};

InCall.propTypes = {
    name: PropTypes.string.isRequired,
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired
};

export default InCall;