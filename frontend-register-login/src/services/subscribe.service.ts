import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SubscribeService {

  private url:string;
  
    constructor(private http:HttpClient) {
      this.url='https://localhost:7080';
     }

    subscribeToCommunity(userID:number,communityID:number){
      return this.http.post(this.url+"/Subscribe/SubscribeUserToCommunity/"+userID+"/"+communityID,{},{responseType:'text'});
    }

    addToModerateCommunity(userID:number,communityID:number){
      return this.http.post(this.url+"/Subscribe/AddUserToModerateCommunity/"+userID + "/"+communityID,{},{responseType:'text'});
    }
     
}
