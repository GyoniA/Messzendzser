import React, {useState} from 'react'
import{useForm} from 'react-hook-form';
import {useNavigate} from "react-router-dom";

export default function Form(){
    const{register, formState:{errors}}= useForm()
    

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    

    let handleSubmit = async (e) => {
        e.preventDefault();
        try {
          let res = await fetch("https://localhost:7043/api/Login", {
            method: "POST",
            mode: 'cors',
              headers: {
                'Access-Control-Allow-Origin': '*',
                username: username,
                password: password,
            },
          });
          let resJson = await res.json();
          if (res.status === 200) {
            setUsername("");
            setMessage("Belépés");
          } else {
            setMessage("Valami baj van :(");
          }
        } catch (err) {
          console.log(err);
        }
      };

    
    let navigate = useNavigate();

    return (
        <section>
            <div className = 'login'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit}>
                        
                        <h1>Bejelentkezés</h1>

                        <label>Felhasználónév / e-mail:</label>
                        <input type="text" value={username} {...register("iden", {required:true})}placeholder='Felhasználónév / e-mail'
                        onChange={(e) => setUsername(e.target.value)}/>
                         {errors.iden?.type ==="required" && "Azonosító megadása kötelező"}
                        
                         
                        <label>Jelszó:</label>
                        <input type="text" value={password} {...register("password", {required:true, maxLength:10})}placeholder='Jelszó'
                        onChange={(e) => setPassword(e.target.value)}/>
                        {errors.password?.type ==="required" && "Jelszó megadása kötelező"}
                        {errors.password?.type ==="maxLength" && "Jelszó túl hosszú"}

                       
                        
                       <div className='row'>
                            <label className='remember'>Emlékezz rám</label>
                            <input className='box' type="checkbox" />
                            <button className='reference' 
                                    onClick={() => {navigate("/register")}}>
                                    Regisztráció
                            </button>

                        </div>

                        <button className='btn'>Belépés</button>

                        <div className="message">{message ? <p>{message}</p> : null}</div>
                    </form> 
            </div>
        </section>
    )
}