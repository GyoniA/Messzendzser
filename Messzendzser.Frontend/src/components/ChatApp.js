import { Component } from "react"
import React from 'react';

//it’ll handle the data and the connection with the API.
import DUMMY_DATA from './DummyData'
import MessageList from './MessageList';

import SendMessageForm from './SendMessageForm';

class ChatApp extends React.Component {
      
    //Set dummydata as state
    constructor() {
        super()
        this.state = {
           messages: DUMMY_DATA
        }
      }

    render() {
      return (
        <div className="chatapp">
            //Next three are the components
            <p>Beszélegtés</p>
          //Pass down messages to MessageList 
          <MessageList messages={this.state.messages}/>
          <SendMessageForm sendMessage={this.sendMessage} />
       </div>
      )
    }

    


//This section needs to be done differently
//WE have to connect to our own API here
//**************************************************************************************************** */

//Sending the message off to Chatkit
//The only parameter is the message text
/*sendMessage(text) {
    this.currentUser.sendMessage({
      text: text,
      roomId: roomId
    })
  }


    //Connecting react Components to API
    componentDidMount() {
        const chatManager = new Chatkit.ChatManager({
          instanceLocator: instanceLocator,
          userId: userId,
          tokenProvider: new Chatkit.TokenProvider({
            url: testToken
          })
       })  
    //Connect with API
    chatManager.connect().then(currentUser => {
        currentUser.subscribeToRoom({
        roomId: roomId,
        hooks: {
          onNewMessage: message => {
            this.setState({
              messages: [...this.state.messages, message]
            })
          }
        }
      })
    })
  }
  //***************************************************************************************************** */

  }
  export default ChatApp;