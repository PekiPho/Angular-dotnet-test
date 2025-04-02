export interface User {
        
        username:string,
        password:string,
        email:string
}

export interface UserFull{
        id:number,
        username:string,
        password:string,
        email:string
}

export interface UserWithoutPass{
        id:number,
        username:string,
        email:string,
}
