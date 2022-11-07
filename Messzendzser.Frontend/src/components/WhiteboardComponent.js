class WhiteboardComponent extends React.Component {
    self = this;
    constructor() {
        super();
    }

    componentDidMount() {
        // Connect
    }

    componentWillUnmount() {
        // Disconnect
    }

    render() {
        return (
        <div className='canvas'>
            <canvas className='wCanvas' id="whiteboardCanvas" width="1000" height="800"></canvas>
        </div>
        );
    }
}

export default WhiteboardComponent;