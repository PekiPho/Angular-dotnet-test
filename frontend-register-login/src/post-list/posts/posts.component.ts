import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Post, PostToSend } from '../../interfaces/post';
import { PostComponent } from '../post/post.component';
import { ActivatedRoute } from '@angular/router';
import { CommunityToSend } from '../../interfaces/community';
import { PostService } from '../../services/post.service';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';

@Component({
  selector: 'posts',
  imports: [PostComponent,NgIf,NgFor],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent implements OnInit{

  public posts:Post[]=[];

  community={} as CommunityToSend;
  
  constructor(private postService:PostService,private communityService:CommunityService){}

  ngOnInit(): void {
    this.communityService.communityy$.subscribe((community:CommunityToSend | null)=>{
      if(community){
        this.community=community;
        this.getPostss();
      }
    })
  }

  

  getPostss(){
    //console.log("hello from posts " + this.community.name);
    if(this.community.name){
      this.postService.getPosts(this.community.name).subscribe({
        next:(data)=>{
          this.posts=data;
          //console.log(this.posts);
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });
    }
    
  }
}
