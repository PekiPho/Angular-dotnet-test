import { Component, Input } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';
import { NgIf } from '@angular/common';
import { PostToSend } from '../../interfaces/post';
import { PostService } from '../../services/post.service';

@Component({
  selector: 'nav-search',
  imports: [RouterLink,RouterModule,NgIf],
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

}
