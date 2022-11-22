import React from "react";
import{useForm} from 'react-hook-form';
import { useNavigate } from "react-router-dom";

export default function Form(){

    let navigate = useNavigate();
    return (
        <section>
            <div id="home">
                <form id='form' className='flex flex-col'>
                    <h1>Messzendzser</h1>
                    <button onClick={() => {navigate("/login")}}>Bejelentkezés</button>
                    <button onClick={() => {navigate("/register")}}>Regisztráció</button>
                    </form> 
                </div>
        </section>
    )
}

