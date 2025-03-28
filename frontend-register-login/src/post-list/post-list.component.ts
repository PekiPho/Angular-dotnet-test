import { Component } from '@angular/core';
import { HistoryComponent } from "./history/history.component";
import { PostsComponent } from "./posts/posts.component";

@Component({
  selector: 'post-list',
  imports: [HistoryComponent, PostsComponent],
  templateUrl: './post-list.component.html',
  styleUrl: './post-list.component.scss'
})
export class PostListComponent {

}
