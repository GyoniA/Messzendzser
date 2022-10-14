import React, { useEffect, useState } from "react";
import {useNavigate} from "react-router-dom";


function Chat(){

    let navigate = useNavigate();

    const [messages, setMessages] = useState([]);
    const [chatrooms, setChatrooms] = useState([]);

    const [message, setMessage] = useState("");
    const [chatroomId, setChatroomId] = useState("");
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
            setMessage("");

            if (resJson.ResponseCode !== 1) {
             
            } 
          }
        } catch (err) {
          console.log(err);
        }
    };
    
    //Load messages from API
    const loadMessages = async (e) => {
        var today = new Date();
        var date = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
        var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
        var dateTime = date + ' ' + time;

        try {
          const res = await fetch("https://localhost:7043/api/GetMessages", {
            method: "GET",
            mode: 'cors',
            credentials: "include",
              headers: {
                'Access-Control-Allow-Origin': '*',
                'count': '20',
                'time': dateTime,
                'dir': "backward"
            },
          });
          let resJson = await res.json();

          if (res.status === 200) {
            setMessages(resJson);

            if (resJson.ResponseCode !== 1) {
             
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
            setChatrooms(resJson);

            if (resJson.ResponseCode !== 1) {
             
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
    }, []);

    /*const displayMessages = messages.map((msg) => {
        if (msg.hasOwnProperty('text')) {
            return(
                <li>
                    {msg.text}
                </li>
            )
        }
        if(msg.hasOwnProperty('length')){
            return(
                <li>
                    <audio controls>
                        <source src="localhost:7043/api/GetVoice?voice=msg.token"
                                type="audio/ogg">
                        </source>
                    </audio>
                </li>
            )
        }else{
            return(
                <li>
                    <img src="localhost:7043/api/GetImage?image=msg.token">
                    </img>
                </li>
            )
        }
    })*/

    return (
        <div className='chatapp'>
            <div className='upper_row'>
                
                <select onChange={(e) => setChatroomId(e.target.value )}
                        value={chatroomId}
                        otions={chatrooms}>
                </select>
                    
                

                <div className='icons_up'>

                    <button className='whiteboard'
                            onClick={() => {navigate("/whiteboard")}}>
                        <img src = "/images/whiteboard.png" ></img>
                    </button> 
                        
                    <button className='phone'>
                        <img src = "/images/phone.png" ></img>
                    </button>
                </div>

            </div>
 
            <div className='middle_part'>

                

                <label className='msg_from_me' >
                    Hello
                </label>
                <label className='msg_from_other' >
                    Szia
                </label>
                <label className='msg_from_me' >
                    Mit csinálsz?
                </label>
                <label className='msg_from_other' >
                    A témalabort
                </label>

            </div>

            <div className='bottom_row'>

                <button className='send'
                    onClick={messageSent}>
                    <img src = "/images/send.png" ></img>
                </button> 

                  
                <input className="msg"
                    type="text"
                    value={message}
                    placeholder='Üzenet írása...'
                    onChange={(e) => setMessage(e.target.value)}>
                </input>
                   

                <button className='microphone'>
                    <img src = "/images/microphone.png" ></img>
                </button> 

                <button className='picture'>
                    <img src = "/images/picture.png" ></img>
                </button>

            </div>

        </div>
    )

}
export default Chat;