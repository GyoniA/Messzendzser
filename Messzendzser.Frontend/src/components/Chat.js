import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";



function Chat() {
    let navigate = useNavigate();

    const [chatroomId, setChatroomId] = useState("");
    const [messages, setMessages] = useState([]);
    const [chatrooms, setChatrooms] = useState([]);
    const [message, setMessage] = useState("");
    const [userId, setUserId] = useState("");

    const messageNum = 20;

    //Send message to API
    let messageSent = async (e) => {
        e.preventDefault();
        try {
            let res = await fetch("https://localhost:7043/api/SendMessage", {
                method: "POST",
                mode: 'cors',
                credentials: "include",

                headers: {
                    'Access-Control-Allow-Origin': '*',
                    message: message,
                    chatroomId: chatroomId,

                },
            });
            let resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setMessage("");
                }
            }
        } catch (err) {
            console.log(err);
        }
    };
    function addZero(i) {
        if (i < 10) { i = "0" + i }
        return i;
    }
    //Load messages from API
    const loadMessages = async (e) => {


        var today = new Date();
        var date = today.getFullYear() + '-' + addZero(today.getMonth() + 1) + '-' + addZero(today.getDate());
        var time = addZero(today.getHours()) + ":" + addZero(today.getMinutes()) + ":" + addZero(today.getSeconds());
        var dateTime = date + ' ' + time;

        try {
            const res = await fetch("https://localhost:7043/api/GetMessages", {
                method: "GET",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    chatroomId: chatroomId,
                    count: 20,
                    time: dateTime,
                    dir: "backward",
                },
            });
            let resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setMessages(resJson.body);
                }

            }
        } catch (err) {
            console.log(err);
        }
    };

    //Load chatrooms from API
    const loadChatrooms = async (e) => {

        try {
            const res = await fetch("https://localhost:7043/api/GetChatrooms", {
                method: "GET",
                mode: 'cors',
                credentials: "include",
                headers: {
                    'Access-Control-Allow-Origin': '*'
                },
            });
            let resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {
                    setChatrooms(resJson.body);
                }
            }
        } catch (err) {
            console.log(err);
        }
    };

    //Run once after the initial render
    useEffect(() => {
        loadMessages();
        loadChatrooms();
        const interval = setInterval(() => {
            loadMessages();
            loadChatrooms();
        }, 3000);
        return () => clearInterval(interval);
    }, [chatroomId]);


    const userIdSet = () => {
        let token = document.cookie;
        token = token.split('.')[1].replace('-', '+').replace('_', '/');
        let decoded = atob(token);

        console.log(decoded);
        console.log(decoded.split('.')[1].replace('-', '+').replace('_', '/'));
        //setUserId(decodedSplit(',')[0]);
        //console.log(userId);
    }

    const displayMessages = () => {
        return messages.map((msg) => {
            if (msg.hasOwnProperty('text')) {
                if (true) {
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
                if (true) {
                    return (
                        <li className="msg_from_me">
                            <audio controls>
                                <source src="localhost:7043/api/GetVoice?voice=msg.token"
                                    type="audio/ogg">
                                </source>
                            </audio>
                        </li>
                    )
                } else {
                    <li className="msg_from_other">
                        <audio controls>
                            <source src="localhost:7043/api/GetVoice?voice=msg.token"
                                type="audio/ogg">
                            </source>
                        </audio>
                    </li>
                }

            } else {
                if (true) {
                    return (
                        <li className="msg_from_me">
                            <img src="localhost:7043/api/GetImage?image=msg.token">
                            </img>
                        </li>
                    )
                } else {
                    return (
                        <li className="msg_from_other">
                            <img src="localhost:7043/api/GetImage?image=msg.token">
                            </img>
                        </li>
                    )
                }

                
            }
        })
    }



    const Chatrooms = () => {
        return chatrooms.map((cr) => {
            return <option key={cr.id} value={cr.id}>{cr.name}
            </option>;
        });
    };

    return (
        userIdSet(),
        <div className='chatapp'>
            <div className='upper_row'>


                <select onChange={(e) => setChatroomId(e.target.value)}>
                    <option value="choose" disabled selected="selected">
                        Név:
                    </option>
                    {Chatrooms()}
                </select>


                <div className='icons_up'>

                    <button className='whiteboard'
                        onClick={() => { navigate("/whiteboard") }}>
                        <img src="/images/whiteboard.png" ></img>
                    </button>

                    <button className='phone'>
                        <img src="/images/phone.png" ></img>
                    </button>
                </div>

            </div>



            <ul>
                {displayMessages()}
            </ul>




            <div className='bottom_row'>

                <button className='send'
                    onClick={messageSent}>
                    <img src="/images/send.png" ></img>
                </button>


                <input className="msg"
                    type="text"
                    value={message}
                    placeholder='Üzenet írása...'
                    onChange={(e) => setMessage(e.target.value)}>
                </input>


                <button className='microphone'>
                    <img src="/images/microphone.png" ></img>
                </button>

                <button className='picture'>
                    <img src="/images/picture.png" ></img>
                </button>

            </div>

        </div>
    )

}
export default Chat;