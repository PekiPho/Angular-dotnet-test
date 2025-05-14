import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Post, PostToSend } from '../interfaces/post';
import { Media } from '../interfaces/media';
import { BehaviorSubject } from 'rxjs';
import { Community } from '../interfaces/community';
import { JsonPipe } from '@angular/common';

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

  addPostWithMedia(username:string,community:string,postJson:any,file:File){
    const formData = new FormData();


    formData.append('postJson',JSON.stringify(postJson));
    if(file){
      formData.append('file',file,file.name);
    }

    return this.http.post(`${this.url}/Post/AddPostWithMedia/${username}/${community}`,formData);
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

  getPostsBySort(communityName:string,type:string,page:number,limit:number,time?:string){
    return this.http.get<Post[]>(`${this.url}/Post/GetPostsBySort/${communityName}/${type}/${page}/${limit}/${time}`);
  }

  getMediaFromPost(postId:string){
    return this.http.get<Media[] | null>(`${this.url}/Media/GetMediaFromPost/${postId}`);
  }

  getPost(postId:string){
    return this.http.get<Post>(`${this.url}/Post/GetPostByID/${postId}`);
  }

  getPostsFromUser(username:string){
    return this.http.get<Post[]>(`${this.url}/Post/GetPostsByUser/${username}`);
  }

  getXVotedPostsFromUser(username:string,vote:boolean){
    return this.http.get<Post[]>(`${this.url}/Post/GetXVotedPostsByUser/${username}/${vote}`);
  }

  searchOnType(query:string){
    return this.http.get<Community[]>(`${this.url}/Search/OnTypeCommunities/${query}`);
  }

  fullSearch(query:string){
    return this.http.get<{communities : Community[],posts: Post[]}>(`${this.url}/Search/GetCommunitiesAndPosts/${query}`);
  }

  deletePost(postId:string){
    return this.http.delete(`${this.url}/Post/DeletePostByName/${postId}`,{responseType:'text'});
  }

  private recentPosts=new BehaviorSubject<Post[]>([]);
  recentPosts$=this.recentPosts.asObservable();

  addToRecent(post:Post){

    //console.log(post);
    
    var curr=this.recentPosts.value.filter(c=>c.id !==post.id);
    curr.unshift(post);

    if(curr.length>25){
      curr.pop();
    }


    this.recentPosts.next(curr);
  }

  clearRecent(){
    this.recentPosts.next([]);
  }

  removeFromRecent(postId:string){
    var updated= this.recentPosts.value.filter(c=>c.id!== postId);
    this.recentPosts.next(updated);
  }
}
