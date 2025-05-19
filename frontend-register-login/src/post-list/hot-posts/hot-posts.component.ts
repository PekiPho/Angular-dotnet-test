import { Component, OnDestroy, OnInit } from '@angular/core';
import { PostComponent } from '../post/post.component';
import { NgFor, NgIf } from '@angular/common';
import { Post } from '../../interfaces/post';
import { PostService } from '../../services/post.service';
import { Subscription } from 'rxjs';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';

@Component({
  selector: 'hot-posts',
  imports: [PostComponent,NgIf,NgFor],
  templateUrl: './hot-posts.component.html',
  styleUrl: './hot-posts.component.scss'
})
export class HotPostsComponent implements OnInit,OnDestroy {

  constructor(
    private postService:PostService,
    private userService:UserServiceService,
  ){}

  public posts:Post[]=[];

  private user:User | null=null;
  private postsSub:Subscription= new Subscription();

  ngOnInit(){
    this.userService.userr$.subscribe((user: User | null)=>{
          if(user){
            this.user=user;
            //console.log(this.user?.username);
            this.postsSub = this.postService.getHotPosts(this.user!.username).subscribe({
              next:(data)=>{
                this.posts=data;

                this.loadMedia();
              },
              error:(err)=>{
                console.log(err);
              }
            });
          }
        });

    
  }

  ngOnDestroy(): void {
    if(this.postsSub)
        this.postsSub.unsubscribe();
  }

  loadMedia(){
    this.posts.forEach((post)=>{
      this.postService.getMediaFromPost(post.id).subscribe({
        next:(data)=>{
          post.mediaIds=data;
        },
        error:(err)=>{
          console.log(err);
        }
      })
    })
  }
}
