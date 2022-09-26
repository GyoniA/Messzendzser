import React, { useState}from 'react'
import{useForm} from 'react-hook-form';
import { useNavigate } from "react-router-dom";



export default function Form(){

    const{register,formState:{errors}}= useForm()

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
            setUsername("");
            setEmail("");
            setMessage("Felhasználó sikeresen létrehozva");
          
          } else {
            setMessage("Sikertelen regisztráció");
          
          }
        } catch (err) {
          console.log(err);
        }
      };

    return (
        <section>
            <div className = 'register'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit}>
                        
                        <h1>Regisztráció</h1>

                        <label>Felhasználónév:</label>
                        <input type="text" value={username} {...register("username", {required:true, maxLength:10})}placeholder='Felhasználónév'
                        onChange={(e) => setUsername(e.target.value)}/>
                         {errors.username?.type ==="required" && "Felhasználónév megadása kötelező"}
                        {errors.username?.type ==="maxLength" && "Felhasználónév túl hosszú"}
                        
                        <label>E-mail:</label>
                        <input type="text" value={email} {...register("email", {required:true,})}placeholder='E-mail' 
                        onChange={(e) => setEmail(e.target.value)}/>
                        {errors.email?.type ==="required" && "E-mail cím megadása kötelező"}

                        <label>Jelszó:</label>
                        <input type="password" value={password} {...register("password", {required:true, maxLength:10})}placeholder='Jelszó'
                        onChange={(e) => setPassword(e.target.value)}/>
                        {errors.password?.type ==="required" && "Jelszó megadása kötelező"}
                        {errors.password?.type ==="maxLength" && "Jelszó túl hosszú"}

                    

                        <button className='btn'
                            //Vár pár másodpercet majd átirányít ha sikeres
                            //onClick={() => setButtonPopup(true)}
                          >Regisztráció</button>
                          
                          <div className="message">{message ? <p>{message}</p> : null}</div>
                    </form> 
            </div>
        </section>
    )
}