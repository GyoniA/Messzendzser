import { Component } from "react"


import React, { useState } from 'react'





class Chat extends React.Component {
    
    constructor(props) {
        super(props)
        this.state = {
            //Here can we load the previous messages
            messages: [],

            //This is the new message what we will send
            message: "",
            chatroomId: "",
            error: null
        }
        this.handleChange = this.handleChange.bind(this);
        this.create = this.create.bind(this);
    }

    

    //HONNAN TUDJUK MELYIK UZENETEK MERT NEM ADTUK MEG A CHATROOM ID-T------ EZZEL AKKOR MIT KELL TENNI????
    //HOGY NZ KI EGY UZENET? UZENET MELYIK ATTRIBUTUMA A SZOVEG???
    //result.messages? result melyik tagja az uzenetek
    componentDidMount() {
        //Get the messages from API 
        fetch("https://localhost:7043/api/GetMessages", {
            method: "GET",
            mode: 'cors',
            headers: {
            'Access-Control-Allow-Origin': '*'  
            }

        })
        .then(res => res.json())
        .then((res) => {
            this.setState({
                messages: res
            });
        })
        .catch(err => {
            console.log(err);
        });
        //Get ChatroomId from API
        fetch("https://localhost:7043/api/GetChatrooms", {
            method: "GET",
            mode: 'cors',
            headers: {
                'Access-Control-Allow-Origin': '*'
            }

        })
        .then(res => res.json())
        .then((res) => {
            this.setState({
                chatroomId: res
            });
        })
        .catch(err => {
            console.log(err);
        });

    }

    //Function to send a message to the API
    create(e) {
        //POST method
        e.preventDefault();

        fetch("https://localhost:7043/api/SendMessage", {
            method: "POST",
            mode: 'cors',
            headers: {
                'Access-Control-Allow-Origin': '*',
                message: this.state.message,
                chatroomId: this.state.chatroomId
            },

        })
            .then(res => res.json())
            .then(res => {
                console.log(res)
            })
            .catch(err => {
                console.log(err);
            });
        


    }
    //Function to update inner state
    handleChange(changeObject) {
        this.setState(changeObject);
    }




    render() {
        
        return (
            <div className='chatapp'>

                <div className='upper_row'>

                    

                    <select
                        value={this.state.chatroomId}
                        onChange={(e) => this.handleChange({ chatroomId: e.target.value })}
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
                        onClick={(e) => {
                            this.create(e);

                        }
                        }>
                        <img src = "/images/send.png" ></img>
                    </button> 

                  
                    <input className="msg"
                        type="text"
                        value={this.state.message}
                        placeholder='Üzenet írása...'
                        onChange={(e) => this.handleChange({ message: e.target.value })}>
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