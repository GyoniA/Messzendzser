import React, { useEffect, useState, useRef, useLayoutEffect } from "react";
import { useNavigate } from "react-router-dom";
import MicRecorder from 'mic-recorder-to-mp3';
import './WhiteBoard.js';
import DecideCall from './DecideCall.js';
import InCall from './InCall.js';
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { wait } from "@testing-library/user-event/dist/utils/index.js";
import VoipComponent from "./VoipComponent.js";

function Chat() {

    //For navigation
    const navigate = useNavigate();
    //States 
    const [chatroomId, setChatroomId] = useState("");

    const refChatroomId = useRef("");

    const refVoiceData = useRef("");

    const [first, setFirst] = useState(true);

    const [messages, setMessages] = useState([]);
    const [chatrooms, setChatrooms] = useState([]);
    const [message, setMessage] = useState("");
    //const [connection, setConnection] = useState < null | HubConnection > (null);
    const [connection, setConnection] = useState();
    const [name, setName] = useState("");

    const [inCall, setInCall] = useState(false);
    const [callFromOther, setCallFromOther] = useState(false);

    const [callFrom, setCallFrom] = useState("");


    const [isRecording, setIsRecording] = useState(false);
    const [voiceData, setVoiceData] = useState("");
    const [Mp3Recorder, setMp3Recorder] = useState(
        new MicRecorder({ bitRate: 128 })
    );

    const MessagesContainer = useRef();




    // Voip

    const voipComp = useRef();

    // Event handlers

    const incomingCallHandler = (from) => {
        console.log(from._display_name);
        setCallFrom(from._display_name);
        console.log("incoming call registered by page, from: " + from);
        setCallFromOther(!callFromOther);
    }

    const callEndedHandler = () => {
        console.log("call ended registered by page");
        setInCall(false);
    }

    const callAcceptedHandler = () => {
        console.log("call accepted registered by page");
        //setCallFromOther(!callFromOther);
    }

    const callFailedHandler = () => {
        console.log("call accepted registered by page");
        setCallFromOther(false);
        setInCall(false);
    }

    // Voip end

    function addZero(i) {
        if (i < 10) { i = "0" + i }
        return i;
    }

    //Load messages from API
    const loadMessages = async (e) => {
        const today = new Date();
        const date = today.getFullYear() + '-' + addZero(today.getMonth() + 1) + '-' + addZero(today.getDate());
        const time = addZero(today.getHours()) + ":" + addZero(today.getMinutes()) + ":" + addZero(today.getSeconds());
        const dateTime = date + ' ' + time;
        try {
            const res = await fetch("https://localhost:7043/api/GetMessages", {
                method: "GET",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    chatroomId: refChatroomId.current,
                    count: 40,
                    time: dateTime,
                    dir: "backward",
                },
            });
            const resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setMessages(resJson.body);
                    MessagesContainer.current.scrollTop = MessagesContainer.current.scrollHeight;
                }

            }
        } catch (err) {
            console.log(err);
        }
    };

    useEffect(() => {
        // TODO retreive voip credentials (maybe store them in cookie or local storage)
        voipSet();
        const name = localStorage.getItem('username');
        const password = localStorage.getItem('voippassword');
        voipComp.current.setCredentials(name, password);
        //voipComp.current.setCredentials("voip", "Password1!");
        voipComp.current.connect();
    }, [])


    useEffect(() => {

        loadChatrooms();
        const connect = new HubConnectionBuilder()
            .withUrl("https://localhost:7043/messageSenderHub")
            .withAutomaticReconnect()
            .build();

        setConnection(connect);

    }, []);



    useEffect(() => {
       
        if (connection) {
            connection
                .start()
                .then(() => {
                    connection.on("ReceiveMessage", () => {
                        loadMessages();
                    });
                })
                .catch((error) => console.log(error));
        }


    }, [connection]);

    const joinRoom = async () => {
        if (connection) await connection.send("JoinRoom", refChatroomId.current);

    };

    const leaveRoom = async () => {
        if (connection) await connection.send("LeaveRoom", refChatroomId.current);
    };

    const sendMessage = async () => {
        if (connection) await connection.send("SendMessage", refChatroomId.current);
    };

    //Send Image to API
    let imageSent = async (e) => {
        const data = new FormData(document.getElementById("uploadImg"));
        try {
            const res = await fetch("https://localhost:7043/api/SendImage", {
                method: "POST",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*',

                    chatroomId: chatroomId,
                },
                body: data,
            });
            const resJson = await res.json();
            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    sendMessage();
                }
            }
        } catch (err) {
            console.log(err);
        }
    };

    //Send Voice to API
    let voiceSent = async (e) => {
        console.log(refVoiceData.current);
        try {
            const res = await fetch("https://localhost:7043/api/SendVoice", {
                method: "POST",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    format: 'MP3',
                    chatroomId: chatroomId,
                    contentType: 'application/my-binary-type',
                    length: 200,
                },
                body: refVoiceData.current,
            });
            const resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    sendMessage();
                }
            }
        } catch (err) {
            console.log(err);
        }
    };


    //Send message to API
    let messageSent = async (e) => {
        try {
            const res = await fetch("https://localhost:7043/api/SendMessage", {
                method: "POST",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    message: message,
                    chatroomId: chatroomId,
                },
            });
            const resJson = await res.json();
            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setMessage("");
                    sendMessage();
                }
            }
        } catch (err) {
            console.log(err);
        }
    };

    //Load chatrooms from API
    const loadChatrooms = async () => {
        try {
            const res = await fetch("https://localhost:7043/api/GetChatrooms", {
                method: "GET",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*'
                },
            });
            const resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setChatrooms(resJson.body);
                    if (first) {
                        setChatroomId(resJson.body[1].id);
                        refChatroomId.current = resJson.body[1].id;
                        joinRoom();
                    }
                    setFirst(false);


                }
            }
        } catch (err) {
            console.log(err);
        }
    };


    useEffect(() => {
        loadChatrooms();
        loadMessages();


    }, [chatroomId]);


    const userIdSet = () => {
        let token;

        var match = document.cookie.match(new RegExp('(^| )' + 'user-token' + '=([^;]+)'));
        if (match)
            token = match[2];

        token = token.split('.')[1].replace('-', '+').replace('_', '/');
        let decoded = atob(token);
        decoded = (decoded.split(',')[0]).split(':')[1];
        let userId = parseInt(decoded);
        localStorage.setItem('userid', userId);
    }

    const voipSet = () => {
        let token;

        var match = document.cookie.match(new RegExp('(^| )' + 'user-token' + '=([^;]+)'));
        if (match)
            token = match[2];
        token = token.split('.')[1].replace('-', '+').replace('_', '/');
        let decoded = atob(token);
        let userName = (decoded.split(',')[1]).split(':')[1];
        let voipPassword = (decoded.split(',')[3]).split(':')[1];
        userName = userName.replaceAll('"', '');
        voipPassword = voipPassword.replaceAll('"', '');
        localStorage.setItem('username', userName);
        localStorage.setItem('voippassword', voipPassword);
    }

    //Display messages in correct form
    const displayMessages = () => {
        return messages.map((msg) => {
            userIdSet();
            const userId = localStorage.getItem('userid');
            if (msg.hasOwnProperty('text')) {
                if (msg.userId == userId) {
                    return (
                        <li className="msg_from_me">
                            {msg.text}
                        </li>
                    )
                } else {
                    return (
                        <li className="msg_from_other">
                            {msg.text}
                        </li>
                    )
                }
            }
            if (msg.hasOwnProperty('length')) {
                if (msg.userId == userId) {
                    return (
                        <li className="voice_from_me">
                            <audio controls>
                                <source src={"https://localhost:7043/api/GetVoice?voice=" + encodeURIComponent(msg.token)}
                                    type="audio/ogg">
                                </source>
                            </audio>
                        </li>
                    )
                } else {
                    return (
                        <li className="voice_from_other">
                            <audio controls>
                                <source src={"https://localhost:7043/api/GetVoice?voice=" + encodeURIComponent(msg.token)}
                                    type="audio/ogg">
                                </source>
                            </audio>
                        </li>
                    )
                }
            } else {
                if (msg.userId == userId) {
                    return (
                        <li className="msg_from_me">
                            <img src={"https://localhost:7043/api/GetImage?img=" + encodeURIComponent(msg.token)}>
                            </img>
                        </li>
                    )
                } else {
                    return (
                        <li className="msg_from_other">
                            <img src={"https://localhost:7043/api/GetImage?img=" + encodeURIComponent(msg.token)}>
                            </img>
                        </li>
                    )
                }
            }
        })
    }

    //When the button is clicked it will trigger the input field
    const hiddenFileInput = React.useRef(null);
    const handleClick = event => {
        event.preventDefault();
        hiddenFileInput.current.click();
    };

    //Function to display only the names
    const Chatrooms = () => {

        return chatrooms.map((cr) => {

            return <option
                key={cr.id} value={cr.id}>{cr.name}
            </option>;
        }

        );

    };

    const btnManager = () => {
        if (!isRecording) {
            startRecording();
        } else {
            stopRecording();
        }
    }

    const startRecording = () => {
        Mp3Recorder
            .start()
            .then(() => {
                setIsRecording(true);
            }).catch((e) => console.error(e));
    };

    const stopRecording = () => {
        Mp3Recorder
            .stop()
            .getMp3()
            .then(([buffer, blob]) => {
                setIsRecording(false);
                refVoiceData.current = blob;
                setVoiceData(blob);
                voiceSent();
            }).catch((e) => console.log(e));
    };



    const handleEnterPressed = (event) => {
        if (event.key === 'Enter') {
            messageSent();
        }
    };








    const hangUp = (e) => {

        setInCall(e);
        voipComp.current.hangUp();

    };

    const popupChangeHandler = (e) => {

        setName(callFrom);
        setCallFromOther(e);
        setInCall(!e);
        voipComp.current.acceptCall();
    };

    const declineCall = (e) => {

        setCallFromOther(e);
        voipComp.current.declineCall();
    };





    return (
        <div className='chatapp'>
            <div className='upper_row'>

                <select id="chatroomSelect" value={chatroomId} onChange={(e) => {
                    if (chatroomId != null) {
                        leaveRoom(chatroomId)
                    }
                    refChatroomId.current = e.target.value;
                    setChatroomId(e.target.value)
                    joinRoom(e.target.value)
                }} >

                    {Chatrooms()};
                </select>

                <VoipComponent uri="wss://localhost:5062/ws" ref={voipComp} callEndedCallback={callEndedHandler} incomingCallCallback={incomingCallHandler} callAcceptedCallback={callAcceptedHandler} callFailedCallback={callFailedHandler} />
                <div className='icons_up'>

                    <button className='whiteboard_button'
                        onClick={() => {
                            let token;
                            var match = document.cookie.match(new RegExp('(^| )' + 'user-token' + '=([^;]+)'));
                            if (match)
                                token = match[2];
                            navigate("/whiteboard", {
                                state: {
                                    chatroomId: chatroomId,
                                    token: token
                                }
                            })
                        }}>

                        <img src="/images/whiteboard.png" ></img>
                    </button>

                    <button className='phone'
                        onClick={(e) => {
                            setInCall(!inCall);
                            let selected = document.getElementById("chatroomSelect");
                            const index = selected.selectedIndex;
                            const tempName = selected.options[index].text;
                            setName(tempName);
                            voipComp.current.call(tempName);
                        }}>
                        <img src="/images/phone.png" ></img>
                    </button>


                </div>
            </div>

            <InCall
                onClose={hangUp}
                show={inCall}
                name={name}>
            </InCall>


            <DecideCall
                onChange={popupChangeHandler}
                onClose={declineCall}
                show={callFromOther}
                name={callFrom}>
            </DecideCall>


            <ul ref={MessagesContainer}>
                {displayMessages()}
            </ul>

            <div className='bottom_row'>

                <button className='send'
                    onClick={messageSent}>
                    <img src="/images/send.png" ></img>
                </button>

                <input className="msg"
                    onKeyPress={handleEnterPressed}
                    type="text"
                    value={message}
                    placeholder='Üzenet írása...'
                    onChange={(e) => setMessage(e.target.value)}>
                </input>

                <button className='microphone'
                    onClick={btnManager}
                    name="voice">
                    <img src="/images/microphone.png" ></img>
                </button>

                <form className="imageSend" id="uploadImg" >
                    <input type="file"
                        ref={hiddenFileInput}
                        onChange={imageSent}
                        name="image"
                        style={{ display: 'none' }} />

                    <button className='picture' onClick={handleClick}>
                        <img src="/images/picture.png" ></img>
                    </button>
                </form>
            </div>
        </div >
    )
}
export default Chat;