import React from 'react'
import{useForm} from 'react-hook-form';

export default function Form(){

    const{register, handleSubmit, formState:{errors}}= useForm()
    const onSubmit = data => console.log(data);

    return (
        <section>
            <div className = 'register'>
                <form id='form' className='flex flex-col' onSubmit={handleSubmit(onSubmit)}>
                        
                        <h1>Regisztráció</h1>

                        <label>Felhasználónév:</label>
                        <input type="text" {...register("username", {required:true, maxLength:10})}placeholder='Felhasználónév'/>
                         {errors.username?.type ==="required" && "Felhasználónév megadása kötelező"}
                        {errors.username?.type ==="maxLength" && "Felhasználónév túl hosszú"}
                        
                        <label>E-mail:</label>
                        <input type="text" {...register("email", {required:true,})}placeholder='E-mail' /*onChange={handleChange}*//>
                        {errors.email?.type ==="required" && "E-mail cím megadása kötelező"}

                        <label>Jelszó:</label>
                        <input type="text" {...register("password", {required:true, maxLength:10})}placeholder='Jelszó'/>
                        {errors.password?.type ==="required" && "Jelszó megadása kötelező"}
                        {errors.password?.type ==="maxLength" && "Jelszó túl hosszú"}
                       
                        <button className='btn'>Regisztráció</button>
                    </form> 
            </div>
        </section>
    )
}