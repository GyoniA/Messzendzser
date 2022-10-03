import React from 'react';

// This component displays an individual message.
// This will display the received messages on the left and, the messages sent by me on the right side
class Message extends React.Component {
  render() {
    const fromMe = this.props.fromMe ? 'from-me' : '';
    return (
        //If the message was sent by my the css class name is message from-me, else just message
      <div className={`message ${fromMe}`}>
        <div className='username'>
          { this.props.username }
        </div>
        <div className='message-body'>
          { this.props.message }
        </div>
      </div>
    );
  }
}

Message.defaultProps = {
};

export default Message;
