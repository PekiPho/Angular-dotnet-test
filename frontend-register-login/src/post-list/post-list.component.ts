import { Component } from '@angular/core';
import { HistoryComponent } from "./history/history.component";
import { PostsComponent } from "./posts/posts.component";
import { HotPostsComponent } from './hot-posts/hot-posts.component';

@Component({
  selector: 'post-list',
  imports: [HistoryComponent, PostsComponent,HotPostsComponent],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent {

}
