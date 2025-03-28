import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Community, CommunityToSend } from '../interfaces/community';
import { BehaviorSubject, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommunityService {

  private url:string;

  constructor(private http:HttpClient) {
    this.url='https://localhost:7080';
   }

   public communitySource=new BehaviorSubject<CommunityToSend | null>(null);
    communityy$= this.communitySource.asObservable();

    setCommunity(community:CommunityToSend){
       this.communitySource.next(community);
    }

   getCommunity(userID:number){

    return this.http.get<Community[]>(this.url+'/Subscribe/GetCommunitiesFromUser/'+userID);
   }

   createCommunity(community:CommunityToSend){
    return this.http.post(this.url+"/Community/CreateCommunity",community);
   }

   getCommunityByName(name:string){
    return this.http.get(this.url+"/Community/GetCommunityByName/"+name);
   }

   getSubscriberCount(name:string){
    return this.http.get(this.url + '/Community/GetSubscribers/'+name);
   }
}
