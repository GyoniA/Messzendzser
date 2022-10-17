import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import recordAudio from './recordAudio';


function Chat() {

    //For navigation
    let navigate = useNavigate();

    //States 
    const [chatroomId, setChatroomId] = useState("");
    const [messages, setMessages] = useState([]);
    const [chatrooms, setChatrooms] = useState([]);
    const [message, setMessage] = useState("");

    var userId;

    const messageNum = 20;




    //Sen Image to API
    let imageSent = async (e) => {
        var data = new FormData(document.getElementById("uploadImg"));
        if (data != null) {
            console.log("ok");
        }
        try {
            let res = await fetch("https://localhost:7043/api/SendImage", {
                method: "POST",
                mode: 'cors',
                data,
                data: new FormData(document.getElementById("uploadImg")),
                headers: {
                    'Access-Control-Allow-Origin': '*',
                   
                    chatroomId: chatroomId,
                },


            });
            let resJson = await res.json();

            if (res.status === 200) {
                if (resJson.message === "Ok") {

                }
            }
        } catch (err) {
            console.log(err);

        }

    };


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

    /*const record = async () => {
        const recorder = await recordAudio();
        recorder.start();
        await sleep(3000);
        const audio = await recorder.stop();
        audio.play();
    };*/

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
        decoded = (decoded.split(',')[0]).split(':')[1];
        userId = parseInt(decoded);

    }

    //Display messages in correct form
    const displayMessages = () => {
        return messages.map((msg) => {
            userIdSet();
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
                if (msg.userId == userId) {
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

    //When the button is clicked it will trigger the input field
    const hiddenFileInput = React.useRef(null);
    const handleClick = event => {
        event.preventDefault();
        hiddenFileInput.current.click();
    };

    //FUnction to display only the names
    const Chatrooms = () => {
        return chatrooms.map((cr) => {
            return <option key={cr.id} value={cr.id}>{cr.name}
            </option>;
        });
    };

    return (


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


                <button className='microphone'
                //onClick={recordAudio()}
                >
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