import React from 'react'
import{useForm} from 'react-hook-form';
import {useNavigate} from "react-router-dom";

export default function Form(){
    const{register, handleSubmit, formState:{errors}}= useForm()
    const onSubmit = data => console.log(data);

    let navigate = useNavigate();

    return (
        <section>
            <div className = 'login'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit(onSubmit)}>
                        
                        <h1>Bejelentkezés</h1>

                        <label>Felhasználónév / e-mail:</label>
                        <input type="text" {...register("iden", {required:true})}placeholder='Felhasználónév / e-mail'/>
                         {errors.iden?.type ==="required" && "Azonosító megadása kötelező"}
                        
                        <label>Jelszó:</label>
                        <input type="text" {...register("password", {required:true, maxLength:10})}placeholder='Jelszó'/>
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
                    </form> 
            </div>
        </section>
    )
}