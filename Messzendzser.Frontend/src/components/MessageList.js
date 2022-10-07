
import React from 'react';

//Takes one prop, messages, which contains an array of objects
//Simpli rendering out text and senderId properties from the object
class MessageList extends React.Component {
    render() {
      return (
        <ul className="message-list">                 
          {this.props.messages.map(message => {
            return (
             <li key={message.id}>
               <div>
                 {message.senderId}
               </div>
               <div>
                 {message.text}
               </div>
             </li>
           )
         })}
       </ul>
      )
    }
  }

  export default MessageList;