
import './css/App.css';
import React from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";

//import React, { Component }  from 'react';
import RegForm from './components/RegForm';
import LogForm from './components/LogForm';
import Home from './components/Home';
import Chat from './components/Chat';
import WhiteBoard from './components/WhiteBoard';
import VoIP from './components/VoIP';


function App() {
    return (
        <Router>
            <nav>
                <Link to="/" style={{ paddingLeft: 5, paddingRight: 10, textDecoration: 'none' }}> Főoldal   </Link>
                <Link to="/register" style={{ paddingRight: 10, textDecoration: 'none' }}> Regisztráció   </Link>
                <Link to="/login" style={{ paddingRight: 10, textDecoration: 'none' }}> Bejelentkezés  </Link>
                <Link to="/whiteboard" style={{ paddingRight: 10, textDecoration: 'none' }}> Whiteboard  </Link>
                <Link to="/voip" style={{ textDecoration: 'none' }}> VoIP  </Link>
            </nav>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/register" element={<RegForm />} />
                <Route path="/login" element={<LogForm />} />
                <Route path="/chat" element={<Chat />} />
                <Route path="/voip" element={<VoIP />} />
                <Route path="/whiteboard" element={<WhiteBoard />} />
            </Routes>
        </Router>
    );
}

export default App;
