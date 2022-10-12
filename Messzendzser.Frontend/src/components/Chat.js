import { Component } from "react"


import React, { useState } from 'react'





class Chat extends React.Component {
    
    constructor(props) {
        super(props)
        this.state = {
            //Here can we load the previous messages
            messages: [],
            messagesLoaded: false,
            chatroomsLoade: false,
            //This is the new message what we will send
            message: "",
            chatroomId: "",
            error: null
        }
        //this.handleChange = this.handleChange.bind(this);
    }

    //Get the messages from the API 
    //HONNAN TUDJUK MELYIK UZENETEK MERT NEM ADTUK MEG A CHATROOM ID-T------ EZZEL AKKOR MIT KELL TENNI????
    //HOGY NZ KI EGY UZENET? UZENET MELYIK ATTRIBUTUMA A SZOVEG???
    //result.messages? result melyik tagja az uzenetek
    componentDidMount() {
        fetch("https://localhost:7043/api/GetMessages")
            .then(res => res.json())
            .then((result) => {
                this.setState({
                    messages: result.messages,
                    messagesLoaded: true
                });
            })
        //Get chatroomId, and the name in it
        //Exception handling!!!!!!
        fetch("https://localhost:7043/api/GetChatrooms")
            .then(res => res.json())
            .then((result) => {
                this.setState({
                    chatroomId: result.chatroomId,
                    chatroomsLoaded: true
                });
            })
        //Exception handling!!!!!!
    }

    handleSubmit = async (e) => {
    e.preventDefault();
    try {
        let res = await fetch("https://localhost:7043/api/SendMessage", {
            method: "POST",
            mode: 'cors',
            headers: {
                'Access-Control-Allow-Origin': '*',
                message: this.state.message,
                chatroomId: this.state.chatroomId,
            },
        });
        let resJson = await res.json();

        if (res.status === 200) {
            //Here we clear the already sent message
            this.state.message=("");
            /*
            if (resJson.ResponseCode !== 1) {
                setMessage("Sikeres belépés");
                setNav(true);
            } else {
                let responseM = resJson.Message;
                setMessage(responseM);
                setNav(false);
            }*/
        }
    } catch (err) {
        console.log(err);
    }
};



    render() {
        const { loaded } = this.state.loaded;
        const { messages } = this.state.messages;
        return (
            <div className='chatapp'>

                <div className='upper_row'>

                    

                    <select
                        value={this.state.chatroomId}
                        onChange={(e) => this.setState({ chatroomId: e.target.value })}
                        >
                        <option>Gábor</option>
                        <option>Gyóni</option>
                        <option>Noi</option>
                       
                    </select>

                    <div className='icons_up'>

                        <button className='whiteboard'>
                            <img src = "/images/whiteboard.png" ></img>
                        </button> 
                        
                        <button className='phone'>
                            <img src = "/images/phone.png" ></img>
                        </button>
                    </div>

                </div>
 
                <div className='middle_part'>
                   
                    <ul>
                        {messages.map(msg => (
                            <li key={msg.}>
                                {msg.}
                            </li>
                            
                        ))}
                    </ul>

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
                        onClick={e => this.handleSubmit}>
                        <img src = "/images/send.png" ></img>
                    </button> 

                  
                    <input className="msg"
                        type="text"
                        value={this.state.message}
                        placeholder='Üzenet írása...'
                        onChange={(e) => this.setState({message: e.target.value})}>
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

}
export default Chat;