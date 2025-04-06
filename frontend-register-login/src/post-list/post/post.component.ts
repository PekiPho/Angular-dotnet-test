import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { Post, PostToSend, Vote } from '../../interfaces/post';
import { formatDate, NgIf } from '@angular/common';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { PostService } from '../../services/post.service';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';

@Component({
  selector: 'post',
  imports: [NgIf],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent implements OnInit{

  constructor(private sanitizer:DomSanitizer,private userService:UserServiceService, private postService:PostService){}

  @Input() public post= {} as Post;

  //public sanitizedUrl:SafeUrl | null = null;

  public isLink:boolean=false;

  private user:User | null = null;

  public voted:boolean|null=null;

  public date:any;

  ngOnInit(): void {

    this.userService.userr$.subscribe((user: User | null)=>{
      if(user){
        this.user=user;
      }
    });

    this.date = this.post.dateOfPost ? (new Date(this.post!.dateOfPost)).toLocaleDateString('en-GB') : null;

    this.postService.getCurrentVote(this.post,this.user!.username).subscribe({
      next:(data)=>{
        if(data!=null)
        {
          if(data)
            this.voted=true;
          else this.voted=false;
        }
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{}
    });

   //console.log(this.post);
    if(this.post.description==null){

    }
    else{
      if(this.checkURL(this.post.description)){
        //console.log("Is url!!");
        this.isLink=true;
        console.log(this.post.mediaIds);
        //this.sanitizedUrl=this.sanitizer.bypassSecurityTrustResourceUrl(this.post.description);
      }
      else{

      }
    }
  }

  checkURL(name:string){
    const regex=/^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/i;

    return regex.test(name);
  }

  voteOnPost(votee:boolean){
    //console.log(votee);
    this.postService.addVote(this.post,this.user!.username,votee).subscribe({
      next:(data)=>{
        console.log(data);
        //this.voted=votee;
        var vote=JSON.parse(data) as Vote;
        this.post.vote= vote.votes;
        
        //console.log(this.voted + " " +votee);
        if(this.voted===votee){
          this.voted=null;
          console.log(this.voted);
        }
        else{
          this.voted=votee;
        }
        
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{}
    });
  }
}
