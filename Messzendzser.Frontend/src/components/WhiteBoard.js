import React, { useEffect, useState } from "react";


function WhiteBoard() {
    const script = document.createElement("script");
    script.src = "js/whiteboard.js";
    script.async = false;
    document.body.appendChild(script);
    return (
        <h1>WhiteBoard</h1>
    );
}
export default WhiteBoard;