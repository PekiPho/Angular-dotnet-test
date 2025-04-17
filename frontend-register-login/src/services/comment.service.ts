import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Comment } from '../interfaces/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  private url:string;
  
    constructor(private http:HttpClient) {
      this.url='https://localhost:7080';
  }


  addComment(username:string,postId:string,replyToId:string,comment:Comment){
    return this.http.post(`${this.url}/Comment/CreateComment/${username}/${postId}/${replyToId}`,comment);
  }

  getCommentsFromPost(postId:string){
    return this.http.get<Comment[]>(`${this.url}/Comment/GetCommentsFromPost/${postId}`);
  }

  getCommentCount(postId:string){
    return this.http.get<number>(`${this.url}/Comment/GetCommentCount/${postId}`);
  }
}
