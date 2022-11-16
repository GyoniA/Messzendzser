import React, { useEffect, useState } from "react";

class VoipComponent extends React.Component {
   

    constructor(props) {        
        super(props);
        // Configs
        this.realm = 'localhost';
        this.expiryTime = 360000;



        this.connectionState = "disconnected";
        this.JsSIP = require('jssip');
        this.md5 = require('md5')
        this.props = props
        this.addedScript = false;
        if (props.uri === undefined) {
            throw new Error("uri is undefined");
        }
        else {
            this.socket = new this.JsSIP.WebSocketInterface(props.uri);
        }
    }

    componentDidMount() {
        // Connect
        if (this.addedScript === false) {
            
        }

    }



    setCredentials = (username, password) => {
        this.username = username;
        this.password = password;
        this.ha1 = this.md5(this.username + ":" + this.realm + ":" + this.password);
    }

    connect = () => {
        if (this.connectionState == "disconnected") {
            this.connectionState = "connecting";
            let configuration = {
                sockets: [this.socket],
                ws_server: 'sip:'+this.realm+':5062',
                display_name: this.username,
                registar_server: 'sip:localhost:5062',
                contact_uri: 'sip:' + this.username + '@' + this.realm,
                hack_ip_in_contact: true,
                authorization_user: this.username,
                uri: 'sip:' + this.username + '@' + this.realm,
                password: this.password,
                register_expires: this.expiryTime,
                ha1: this.ha1
            };

            this.ua = new this.JsSIP.UA(configuration);
            this.ua.on('connected', function (e) {
                console.log("connected")
                this.connectionState = "connected";
            });
            this.ua.on('disconnected', function (e) {
                console.log("disconnected")
                this.connectionState = "disconnected";
            });
            this.ua.on('newRTCSession', function (e) {
                let session = e.session; 
                if (e.originator == 'remote') { // outgoing call session here
                    console.log("Incoming call from: " + e.request.from);
                    
                    if (this.incomingCallSession !== undefined || this.activeCallSession !== undefined) {
                        console.log("rejecting incoming call because other session is in progress (either incoming or active)");
                        e.session.terminate();
                    }
                    else {
                        // TODO add ringtone
                        e.session.on("confirmed", function () {
                            let voipAudio = document.getElementById("voipAudio")
                            voipAudio.srcObject = session.connection.getRemoteStreams()[0];
                            voipAudio.play();
                            console.log("call confirmed");
                        });
                        this.incomingCallSession = e.session;
                        this.props.incomingCallCallback(e.request.from);
                    }
                }
            });
            this.ua.on('registered', function (e) {
                this.connectionState = "registered";
                console.log("registered");
            });
            this.ua.on('unregistered', function (e) { console.log("unregistered") });
            this.ua.on('registrationFailed', function (e) { console.log("registration failed") });

            this.ua.start();
        }
    }

    // Starts a new call
    call = (username) => {
        let eventHandlers = {
            'progress': function (data) { console.log("call progress") },
            'failed': function (data) {
                console.log("call failed")
                this.props.callFailedCallback();
            },
            'confirmed': function (data) {
                console.log("call confirmed")
                this.props.callAcceptedCallback();
            },
            'ended': function (data) {
                console.log("call ended")
                this.props.callEndedCallback();
            }
        };

        let options = {
            mediaConstraints: {
                audio: true,
                video: false
            }
        };
        console.log("calling sip:" + username + '@' + this.realm);
        this.ua.call('sip:' + username + '@' + this.realm, options);
    }

    // Declines an incoming call
    declineCall = () => {
        if (this.incomingCallSession !== undefined) {
            this.incomingCallSession.terminate();
            this.incomingCallSession = undefined;
        }
    }

    // ends an active call
    hangUp = () => {
        if (this.activeCallSession !== undefined) {
            this.activeCallSession.terminate();
            this.activeCallSession = undefined;
        }
    }

    // Accepts incoming call
    acceptCall = () => {
        if (this.incomingCallSession !== undefined) {
            this.incomingCallSession.answer();
            this.activeCallSession = this.incomingCallSession;
            this.incomingCallSession = undefined;
            this.incomingCallSession.on('addstream', function (i) {
                console.log("stream added to call");
                let voipAudio = document.getElementById("voipAudio")
                voipAudio.src = window.URL.createObjectURL(i.stream);
                voipAudio.play();
            });
        }
    }

    componentWillUnmount() {
        // Disconnect
    }



    render() {
        return (
            <div id='voipContainer'>
                <script src="js/jssip.js"></script>
                <audio id="voipAudio"></audio>
            </div>
        );
    }
}

export default VoipComponent;