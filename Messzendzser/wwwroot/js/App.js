
import './App.css';
import React from "react";
import {BrowserRouter as Router, Routes, Route, Link} from "react-router-dom";

//import React, { Component }  from 'react';
import RegForm from './components/RegForm';
import LogForm from './components/LogForm';
import Home from './components/Home';
 

function App() {
  return (
  <Router>
    <nav>
      <Link to="/"> Home </Link>
      <Link to="/register"> Register </Link>
      <Link to="/login"> Login </Link>
    </nav>
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/register" element={<RegForm />} />
      <Route path="/login" element={<LogForm />} />
    </Routes>
  </Router>
  );
}

export default App;
