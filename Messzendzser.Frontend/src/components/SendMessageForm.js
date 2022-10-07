class SendMessageForm extends React.Component {

    //Initialize state and bind this in the handleChange method
    constructor() {
        super()
        this.state = {
           message: ''
        }
        this.handleChange = this.handleChange.bind(this)
        this.handleSubmit = this.handleSubmit.bind(this)
    }

    render() {
      return (
        <form
        onSubmit={this.handleSubmit}
          className="send-message-form">
          <input
          //Listening to inputs, trigger handleChange 
            onChange={this.handleChange}
            //setting the value of the message
            value={this.state.message}
            placeholder="Type your message and hit ENTER"
            type="text" />
        </form>
      )
    }

    //Update the state to have the newest value
    handleChange(e) {
        this.setState({
          message: e.target.value
        })
      }

      //We pass the current value (message) along with the submission
      handleSubmit(e) {
        e.preventDefault()
        this.props.sendMessage(this.state.message)
        //Clearing out the input field
        this.setState({
          message: ''
        })
      }

  }
  export default SendMessageForm;