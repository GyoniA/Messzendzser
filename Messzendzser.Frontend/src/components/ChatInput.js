import React from 'react';

class ChatInput extends React.Component {
  constructor(props) {
    super(props);
    this.state = { chatInput: '' };

    this.submitHandler = this.submitHandler.bind(this);
    this.textChangeHandler = this.textChangeHandler.bind(this);
  }
  
  submitHandler(event) {
    event.preventDefault();
    this.setState({ chatInput: '' });
    this.props.onSend(this.state.chatInput);
  }

  textChangeHandler(event)  {
    this.setState({ chatInput: event.target.value });
  }

  render() {
    return (
      <form className="chat-input" onSubmit={this.submitHandler}>
        <input type="text"
          onChange={this.textChangeHandler}
          value={this.state.chatInput}
          placeholder="Írj üzenetet..."
          required />
      </form>
    );
  }
}

ChatInput.defaultProps = {
};

export default ChatInput;