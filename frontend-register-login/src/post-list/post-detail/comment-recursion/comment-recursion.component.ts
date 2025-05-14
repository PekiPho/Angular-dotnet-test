import { Component, Input, input, NgModule, OnChanges, OnInit, SimpleChanges,ElementRef,ViewChild, Output, EventEmitter, AfterViewInit, ChangeDetectorRef, OnDestroy, HostListener } from '@angular/core';
import { Comment } from '../../../interfaces/comment';
import { NgFor, NgIf, NumberSymbol } from '@angular/common';
import {format} from 'date-fns';
import { CommentService } from '../../../services/comment.service';
import { UserServiceService } from '../../../services/user-service.service';
import { User } from '../../../interfaces/user';
import { PostService } from '../../../services/post.service';
import { Route, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { last, max } from 'rxjs';

@Component({
  selector: 'comment-recursion',
  imports: [NgIf,NgFor,FormsModule],
  templateUrl: './comment-recursion.component.html',
  styleUrl: './comment-recursion.component.scss'
})
export class CommentRecursionComponent implements OnInit,OnChanges,AfterViewInit,OnDestroy{

  constructor(private commentService:CommentService,
      private userService:UserServiceService,
      private postService:PostService,
      private route:Router,
      private el:ElementRef,
      private cdRef:ChangeDetectorRef,
  ){}

  @Input() comment:Comment={};
  @Input() level:number=-1;

  public user={} as User;

  public date:any;
  public voted:boolean | null = null;
  public visible:boolean=false;
  public showMenu:boolean=false;


  public showReply:boolean=false;
  public replyContent:string='';

  public lineHeight:number=0;
  public collapsed:boolean=false;

  private resizeObserver!: ResizeObserver;

  ngOnInit(): void {
    //console.log(this.comment.dateOfComment);
    this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');

    this.userService.userr$.subscribe({
      next:(data)=>{
        this.user= data as User;
      },
      error:(err)=>{
        console.log(err);
      }
    });


    this.commentService.getVoteValue(this.comment.id!,this.user.username).subscribe({
      next:(data)=>{
        this.voted=data;
      }
    })

    //this.getComment();
    //this.calcLineHeight();
    //this.calculateBottom();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['comment']){
      this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');
      //this.calculateBottom();
    }
  }

  ngAfterViewInit(): void {
    

    this.resizeObserver=new ResizeObserver(()=> {
      this.calculateLineHeight();
      this.cdRef.detectChanges();
    }
  );

    this.resizeObserver.observe(this.el.nativeElement);

    this.calculateLineHeight();
    this.cdRef.detectChanges();
  }

  ngOnDestroy(): void {
    if(this.resizeObserver)
      this.resizeObserver.disconnect();
  }

  voteOnComment(value:boolean){
    //this.voted=value;

    this.commentService.addCommentVote(this.comment.id!,this.user.username,value).subscribe({
      next:(data)=>{
        //var votee=JSON.parse(data) as boolean | null;
        if(data===null){
          if(this.voted)
            this.comment.vote!--;
          else this.comment.vote!++;

          this.voted=null;
        }  
        else{
          if(this.voted===null){
           if(data)
            this.comment.vote!++;
          else this.comment.vote!--;
        }
        else{
          if(data)
            this.comment.vote!+=2;
          else this.comment.vote!-=2;
        }

        this.voted=data as boolean;
        }
    }});
  }

  toggleReply(){
    this.showReply=!this.showReply;

    if (!this.showReply) this.replyContent = '';
  }

  submitReply(){
    if(this.replyContent!=''){

      var seg=this.route.url.split('/');

    var postId=seg[seg.length-1];

    var comm={}as Comment;

    comm.content=this.replyContent;
    comm.username=this.user.username;
    comm.postId=postId;
    comm.replyToId=this.comment.id;

    this.commentService.addComment(this.user.username,postId,comm.replyToId!,comm).subscribe({
      next:(data)=>{
          location.reload();
      },
      error:(err)=>{
        console.log(err);
      }
    });
    }
    
  }

  getWidth(level:number){
    if(level===0)
      return '100%';
    else return `calc(100% - 20px)`;
  }

  calculateLineHeight(){
    
    var childEl=this.el.nativeElement.querySelectorAll(':scope > div > div > div > comment-recursion');
    var child=this.el.nativeElement;

    if(childEl.length===0){
      this.lineHeight=this.el.nativeElement.getBoundingClientRect().height-14;
      return;
    }
    
    var lastEl=childEl[childEl.length-1];
    this.lineHeight=lastEl.getBoundingClientRect().top-this.el.nativeElement.getBoundingClientRect().top-14;
  }

  calcMargin(){
    if(this.level===0){
      return 0;
    }
    else return 20;
  }

  @Output() collapseChanged=new EventEmitter<void>();

  changeCollapse(){
    this.collapsed=!this.collapsed;
    //this.calculateLineHeight();
    this.collapseChanged.emit();
  }

  onChildResized(){
    this.calculateLineHeight();

    //console.log(this.lineHeight);

    this.collapseChanged.emit();
  }

  @HostListener('document:click',['$event'])
  handleClick(event:MouseEvent){
    var target = event.target as HTMLElement;

    if(!target.closest('.dropdown')){
      this.showMenu=false;
    }
  }

  deleteComment(){
    this.showMenu=false;


    this.commentService.deleteComment(this.comment.id!).subscribe({
      next:(data)=>{
        location.reload();
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }

}



