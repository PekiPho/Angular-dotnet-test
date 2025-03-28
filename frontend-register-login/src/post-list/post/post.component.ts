import { Component, Input, OnInit } from '@angular/core';
import { Post, PostToSend } from '../../interfaces/post';
import { NgIf } from '@angular/common';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'post',
  imports: [NgIf],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent implements OnInit{

  constructor(private sanitizer:DomSanitizer){}

  @Input() public post= {} as Post;

  public sanitizedUrl:SafeUrl | null = null;

  public isLink:boolean=false;

  ngOnInit(): void {
    console.log(this.post);
    if(this.post.description==null){

    }
    else{
      if(this.checkURL(this.post.description)){
        console.log("Is url!!");
        this.isLink=true;

        this.sanitizedUrl=this.sanitizer.bypassSecurityTrustResourceUrl(this.post.description);
      }
      else{

      }
    }
  }

  checkURL(name:string){
    const regex=/^(https?:\/\/[^\s]+)$/;

    return regex.test(name);
  }

  upvotePost(){

  }

  downvotePost(){
    
  }
}
