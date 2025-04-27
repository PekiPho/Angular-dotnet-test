import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { PostService } from '../../services/post.service';
import { Post } from '../../interfaces/post';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'history',
  imports: [NgIf,NgFor,RouterModule],
  templateUrl: './history.component.html',
  styleUrl: './history.component.scss'
})
export class HistoryComponent implements OnInit {

  public recentPosts:Post[]=[];

  constructor(
    private route:Router,
    private postService:PostService,
  ){}

  ngOnInit(): void {
    this.postService.recentPosts$.subscribe({
      next:(data)=>{
        this.recentPosts=data;
        console.log(data);
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }

  clearRecent(){
    this.postService.clearRecent();
  }
}
