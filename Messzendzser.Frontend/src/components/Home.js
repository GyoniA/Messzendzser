import React from "react";
import{useForm} from 'react-hook-form';
import { useNavigate } from "react-router-dom";

export default function Form(){

    let navigate = useNavigate();
    return (
        <section>
            <div className="home">
                <form id='form' className='flex flex-col'>
                    <h1>Messzendzser</h1>
                    <button className='login_btn'onClick={() => {navigate("/login")}}>Bejelentkezés</button>
                    <button className='register_btn'onClick={() => {navigate("/register")}}>Regisztráció</button>
                    </form> 
                </div>
        </section>
    )
}

