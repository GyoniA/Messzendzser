import React, { useState } from 'react'
import { useForm } from 'react-hook-form';
import { useNavigate } from "react-router-dom";

export default function Form() {



    const { register, formState: { errors } } = useForm()


    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");




    let navigate = useNavigate();

    let handleSubmit = async (e) => {
        e.preventDefault();
        try {
          let res = await fetch("https://localhost:7043/api/Login", {
              method: "POST",
              credentials: 'include',
            mode: 'cors',
              headers: {
                'Access-Control-Allow-Origin': '*',
                username: username,
                password: password,
            },
          });
          let resJson = await res.json();

            if (res.status === 200) {


                if (resJson.message === "Ok") {

                    setUsername("");
                    setPassword("");
                    let tok = resJson.body.token;
                    document.cookie = "user-token=" + resJson.body.token;

                    navigate("/chat")

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
            <div className='login'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit}>

                    <h1>Bejelentkezés</h1>

                    <label>Felhasználónév / e-mail:</label>
                    <input type="text" value={username} required placeholder='Felhasználónév / e-mail'
                        onChange={(e) => setUsername(e.target.value)} />



                    <label>Jelszó:</label>
                    <input type="password" value={password} required placeholder='Jelszó'
                        onChange={(e) => setPassword(e.target.value)} />




                    <div className='row'>

                        <button type="button" className='reference'
                            onClick={() => { navigate("/register") }}>
                            Regisztráció
                        </button>

                    </div>

                    <button type="submit" className='btn'>Belépés</button>

                    <div className="message">
                        {message ? <p>{message.username}</p> : null}
                    </div>
                </form>
            </div>
        </section>
    )
}