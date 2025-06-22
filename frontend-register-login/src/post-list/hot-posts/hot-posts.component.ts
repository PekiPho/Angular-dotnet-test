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
  public page:number=1;
  public isLoading:boolean=false;
  public hasMore:boolean=true;

  private user:User | null=null;
  private postsSub:Subscription= new Subscription();

  ngOnInit(){
    this.userService.userr$.subscribe((user: User | null)=>{
          if(user){
            this.user=user;
            //console.log(this.user?.username);
            this.loadPosts();
          }
        });

    
  }

  ngOnDestroy(): void {
    if(this.postsSub)
        this.postsSub.unsubscribe();
  }

  loadPosts(){
    if(this.isLoading || !this.hasMore) return;

    this.isLoading=true;

    this.postsSub = this.postService.getHotPosts(this.user!.username,this.page).subscribe({
      next:(data)=>{
        if(data.length<50)
          this.hasMore=false;
        
        this.posts=[...this.posts,...data];

        this.loadMedia();
        this.page++;

        this.isLoading=false;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
      }
    });
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

  onScroll(event:Event){
    const element = event.target as HTMLElement;
    const threshold = 150;
    const scrollTop = element.scrollTop;
    const clientHeight = element.clientHeight;
    const scrollHeight = element.scrollHeight;

    if (scrollTop + clientHeight >= scrollHeight - threshold && !this.isLoading && this.hasMore) {
      this.loadPosts();
    }
  }
}
