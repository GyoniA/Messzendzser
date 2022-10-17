import React from "react";

function VoIP() {
    const script1 = document.createElement("script");
    script1.src = "js/jssip.js";
    script1.async = false;
    document.body.appendChild(script1);
    const script2 = document.createElement("script");
    script2.src = "js/voipController.js";
    script2.async = false;
    document.body.appendChild(script2);
    return (
        <div>
            <h1>VoIP</h1>
        </div>
    );
}
export default VoIP;