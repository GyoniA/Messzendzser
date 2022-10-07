
import './css/App.css';
import React from "react";
import {BrowserRouter as Router, Routes, Route, Link} from "react-router-dom";

//import React, { Component }  from 'react';
import RegForm from './components/RegForm';
import LogForm from './components/LogForm';
import Home from './components/Home';
import Chat from './components/Chat';
 

function App() {
  return (
  <Router>
    <nav>
      <Link to="/"> Főoldal   </Link>
      <Link to="/register"> Regisztráció   </Link>
      <Link to="/login"> Bejelentkezés  </Link>
      <Link to="/chat"> Beszélgetés  </Link>
    </nav>
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/register" element={<RegForm />} />
      <Route path="/login" element={<LogForm />} />
      <Route path="/chat" element={<Chat />} />
    </Routes>
  </Router>
  );
}

export default App;
