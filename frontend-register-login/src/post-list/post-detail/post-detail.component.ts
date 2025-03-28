import { Component } from '@angular/core';
import { CommunityInfoComponent } from "../community-info/community-info.component";

@Component({
  selector: 'post-detail',
  imports: [CommunityInfoComponent],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.scss'
})
export class PostDetailComponent {

}
