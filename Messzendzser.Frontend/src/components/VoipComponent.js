import React, { useEffect, useState, useRef } from "react";
import VoIP from "./VoIP";

class VoipComponent extends React.Component {

    

    constructor(props) {        
        super(props);
        // Configs
        this.realm = 'localhost';
        this.expiryTime = 360000;

        var self = this;

        this.connectionState = "disconnected";
        this.JsSIP = require('jssip');
        //this.JsSIP.debug.enable('JsSIP:*');
        this.JsSIP.debug.disable();
        this.md5 = require('md5')
        this.props = props
        this.addedScript = false;

        this.Ringtone = React.createRef();
        if (props.uri === undefined) {
            throw new Error("uri is undefined");
        }
        else {
            this.socket = new this.JsSIP.WebSocketInterface(props.uri);
        }
    }

    componentDidMount() {
        
    }

    playRingtone = () => {
        this.Ringtone.current.play();
    }

    stopRingtone = () => {
        this.Ringtone.current.pause();
        this.Ringtone.current.currentTime = 0;
    }

    setCredentials = (username, password) => {
        this.username = username;
        this.password = password;
        this.ha1 = this.md5(this.username + ":" + this.realm + ":" + this.password);
    }

    connect = () => {
        var self = this;
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

            self.ua = new self.JsSIP.UA(configuration);
            self.ua.on('connected', function (e) {
                console.log("connected")
                self.connectionState = "connected";
            });
            self.ua.on('disconnected', function (e) {
                console.log("disconnected")
                self.connectionState = "disconnected";
            });
            self.ua.on('newRTCSession', function (e) {
                let session = e.session;
                session.on('ended', function (e) {
                    console.log('call ended');
                    self.props.callEndedCallback();
                    self.incomingCallSession = undefined;
                    self.activeCallSession = undefined;
                });
                session.on('failed', function (e){
                    console.log("call failed")
                    self.props.callFailedCallback();
                    self.incomingCallSession = undefined;
                    self.activeCallSession = undefined;
                    self.stopRingtone();

                });
                if (e.originator == 'remote') { // outgoing call session here
                    console.log("Incoming call from: " + e.request.from);

                    if (self.incomingCallSession !== undefined || self.activeCallSession !== undefined) {
                        console.log("rejecting incoming call because other session is in progress (either incoming or active)");
                        e.session.terminate();
                    }
                    else {
                        self.playRingtone();
                        e.session.on("confirmed", function () {
                            self.stopRingtone();
                            let voipAudio = document.getElementById("voipAudio")
                            voipAudio.srcObject = session.connection.getRemoteStreams()[0];
                            voipAudio.play();
                            console.log("call confirmed");
                        });
                        self.incomingCallSession = e.session;
                        self.props.incomingCallCallback(e.request.from);
                    }
                } else {
                    self.activeCallSession = session;
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
        let self = this;
        let eventHandlers = {
            'progress': function (data) { console.log("call progress") },
            'confirmed': function (data) {
                console.log("call confirmed")
                self.props.callAcceptedCallback();
            }
        };

        let options = {
            'eventHandlers': eventHandlers,
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
            this.stopRingtone();
            this.incomingCallSession.terminate();
            this.incomingCallSession = undefined;
        }
    }

    // ends an active call
    hangUp = () => {
        if (this.activeCallSession !== undefined) {
            console.log("hanging up");
            this.activeCallSession.terminate();
            this.activeCallSession = undefined;
        }
    }

    // Accepts incoming call
    acceptCall = () => {
        let self = this;
        if (this.incomingCallSession !== undefined) {
            let voipAudio = document.getElementById("voipAudio")
            self.stopRingtone();
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
                <audio id="ringtone" loop={true} src="/audio/ringtone.mp3" ref={this.Ringtone}></audio>
            </div>
        );
    }
}

export default VoipComponent;