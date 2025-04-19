import { Component, Input, input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Comment } from '../../../interfaces/comment';
import { NgFor, NgIf } from '@angular/common';
import {format} from 'date-fns';

@Component({
  selector: 'comment-recursion',
  imports: [NgIf,NgFor],
  templateUrl: './comment-recursion.component.html',
  styleUrl: './comment-recursion.component.scss'
})
export class CommentRecursionComponent implements OnInit,OnChanges{

  @Input() comment:Comment={};
  @Input() level:number=-1;

  public date:any;
  public voted:boolean | null = null;

  ngOnInit(): void {
    console.log(this.comment.dateOfComment);
    this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['comment']){
      this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');
    }
  }

  voteOnComment(value:boolean){
    this.voted=value;
  }

}
