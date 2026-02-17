import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User, UserFull } from '../interfaces/user';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserServiceService {

  private url:string ;

  constructor(private http: HttpClient) { 
    this.url = 'https://localhost:7080';
  }

  public userSource=new BehaviorSubject<User | null>(null);
  userr$=this.userSource.asObservable();
  
  setUser(user:User){
    this.userSource.next(user);
  }

  

  getUsers(){
    return this.http.get<User>(this.url + '/Ispit/GetUsers');
  }

  createUser(user: User) {
    return this.http.post<{token: string, expiration: string}>(this.url + '/Ispit/AddUser', user);
  }

  deleteUser(id:number){
    return this.http.delete(this.url + '/Ispit/DeleteUser/'+ id)
  }

  checkLogin(mail:string,pass:string){
    
    return this.http.get(this.url+ "/Ispit/GetUserByMailAndPassword/"+mail + "/"+ pass,{responseType:'json'})
    .pipe(
      tap((data:any)=>{
        if(data.token){
          localStorage.setItem('token',data.token);
          //console.log(data.token);
        }
      })
    );
  }
  


  //jwt test
  getEntry(){
    const token=localStorage.getItem('token');

    if(!token)
      throw new Error("No token found");
    else{
      const headers=new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      return this.http.get<any>(this.url+"/Ispit/GetEntry",{headers});
    }
  }
}
