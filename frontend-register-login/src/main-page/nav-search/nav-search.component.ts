import { Component, Input } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';
import { NgFor, NgIf } from '@angular/common';
import { Post, PostToSend } from '../../interfaces/post';
import { PostService } from '../../services/post.service';
import { Community } from '../../interfaces/community';

@Component({
  selector: 'nav-search',
  imports: [RouterLink,RouterModule,NgIf,NgFor],
  templateUrl: './nav-search.component.html',
  styleUrl: './nav-search.component.scss'
})
export class NavSearchComponent {

  constructor(private postService:PostService){}

    @Input() user?:string;

    public createPost:boolean=false;

    showPostModal(){
      this.createPost=true;

      //console.log(this.createPost);
    }

    disablePostModal(){
      this.createPost=false;
    }

    public text:boolean=true;
    public imgVid:boolean=false;
    public link:boolean=false;

    textClick(){
      this.text=true;
      this.imgVid=false;
      this.link=false;
    }

    imgVidClick(){
      this.text=false;
      this.imgVid=true;
      this.link=false;
    }

    linkClick(){
      this.text=false;
      this.imgVid=false;
      this.link=true;
    }


    addPost(){
      var title=(document.querySelector("#title") as HTMLInputElement).value;
      var community=(document.querySelector("#community") as HTMLInputElement).value;

      var post:PostToSend= {} as PostToSend;

      //post=new PostToSend();

      post.title=title;

      if(this.text){
        var desc=(document.querySelector("#description")as HTMLTextAreaElement).value;

        post.description=desc;
      }
      if(this.imgVid){
        var imageVideo=(document.querySelector("#imgVideo") as HTMLInputElement).files;

        if(imageVideo!=null){
          for(var i=0;i<imageVideo.length;i++){

            const file=imageVideo[i];
            if(file.type.startsWith('video/')){

              const video=document.createElement('video');
              video.preload='metadata';

              video.onloadedmetadata=function(){
                var duration=video.duration;
                if(duration>15*60)
                {
                  console.log("File too long");
                  return;
                }
              }
            }
          }
        }

        
        
      }
      if(this.link){
        var desc=(document.querySelector("#link")as HTMLInputElement).value;

        post.description=desc;
      }

      //console.log(post);

      this.postService.addPost(this.user!,community,post).subscribe({
        next:(data)=>{
          console.log(data);
          window.location.reload();
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });

    }

    public communities:Community[]=[];
    //public posts:Post[]=[];
    public showSug:boolean=false;

    onType(event:Event){
      //var input=document.querySelector("#search") as HTMLInputElement;
      var input=event.target as HTMLInputElement;
      this.query=input.value;


      if(input.value.length>3){
        this.communities=[];
        //this.posts=[];

        this.postService.searchOnType(input.value).subscribe({
          next:(data)=>{
            //console.log(data);
            this.communities=data;
            this.showSug=true;
          },
          error:(err)=>{
            console.log(err);
          }
        });
      }
      else{
        this.showSug=false;
      }
    }

    public query:string='';

    onSearch(input:HTMLInputElement){

      this.communities=[];
      
      // this.posts=[];

      // this.postService.fullSearch(input.value).subscribe({
      //   next:(data)=>{
      //     this.communities=data.communities;
      //     this.posts=data.posts;

      //     //console.log(this.communities);
      //     //console.log(this.posts);
      //     input.value='';
      //   },
      //   error:(err)=>{
      //     console.log(err);
      //   }
      // });

      //setTimeout(() => this.clearInput(input), 100);
    }

    clearInput(input:HTMLInputElement){
      setTimeout(() => {
        input.value='';
        this.showSug=false;
      }, 500);
      
    }

}
