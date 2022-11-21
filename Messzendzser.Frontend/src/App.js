
import './css/App.css';
import React from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";

//import React, { Component }  from 'react';
import RegForm from './components/RegForm';
import LogForm from './components/LogForm';
import Home from './components/Home';
import Chat from './components/Chat';
import WhiteBoard from './components/WhiteBoard';


function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/register" element={<RegForm />} />
                <Route path="/login" element={<LogForm />} />
                <Route path="/chat" element={<Chat />} />
                <Route path="/whiteboard" element={<WhiteBoard />} />
            </Routes>
        </Router>
    );
}

export default App;
