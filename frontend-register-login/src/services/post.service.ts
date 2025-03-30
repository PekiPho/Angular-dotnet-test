import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Post, PostToSend } from '../interfaces/post';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  
  private url:string;

  constructor(private http:HttpClient) {
    this.url='https://localhost:7080';
   }




  addPost( username:string,community:string,post:PostToSend){
    return this.http.post(this.url+"/Post/AddPostByName/"+username+"/"+community,post,{responseType:'json'});
  }

  getPosts(communityName:string){
    return this.http.get<Post[]>(this.url+"/Post/GetPostsFromCommunity/"+communityName);
  }

  addVote(post:Post,username:string,vote:boolean){
    return this.http.post(`${this.url}/Vote/AddVote/${post.id}/${username}/${vote}`,{},{responseType:'text'});
  }

  getCurrentVote(post:Post,username:string){
    return this.http.get(`${this.url}/Vote/GetCurrentVote/${post.id}/${username}`);
  }
}
