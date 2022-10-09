import { Component } from "react"
import React from 'react';
import TextField from '@material-ui/core/TextField';
import Autocomplete from '@material-ui/lab/Autocomplete';





class Chat extends React.Component {
    
    

    render() {
        return (
            <div className='chatapp'>

                <div className='upper_row'>

                    

                    <select>
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

                    <button className='send'>
                        <img src = "/images/send.png" ></img>
                    </button> 

                    <input className="msg"
                        type="text" 
    
                        placeholder='Üzenet írása...'>
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