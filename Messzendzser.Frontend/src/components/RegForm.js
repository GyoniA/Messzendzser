import React, { useState } from 'react'
import { useForm } from 'react-hook-form';
import { useNavigate } from "react-router-dom";



export default function Form() {

    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");


    let navigate = useNavigate();


    let handleSubmit = async (e) => {
        e.preventDefault();
        try {
            let res = await fetch("https://localhost:7043/api/Register", {
                method: "POST",
                mode: 'cors',
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    username: username,
                    email: email,
                    password: password,
                },
            });
            let resJson = await res.json();

            if (res.status === 200) {


                if (resJson.message === "Ok") {
                    setUsername("");
                    setPassword("");
                    setEmail("");
                    navigate("/login");

                } else {
                    let responseM = resJson.errors;
                    setMessage(responseM);
                }

            }
        } catch (err) {
            console.log(err);
        }
    };



    return (
        <section>
            <div className='register'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit}>

                    <h1>Regisztráció</h1>

                    <label>Felhasználónév:</label>
                    <input type="text" value={username} required placeholder='Felhasználónév'
                        onChange={(e) => setUsername(e.target.value)} />

                    <label>E-mail:</label>
                    <input type="text" value={email} required placeholder='E-mail'
                        onChange={(e) => setEmail(e.target.value)} />

                    <label>Jelszó:</label>
                    <input type="password" value={password} required placeholder='Jelszó'
                        onChange={(e) => setPassword(e.target.value)} />



                    <button type="submit" className='btn'>
                        Regisztráció</button>

                    <div id="error_message">
                        <p>{message.username}</p>
                        <p>{message.email}</p> 
                        <p>{message.password}</p>
                        <p>{message.error}</p>
                    </div>
                </form>
            </div>
        </section>
    )
}