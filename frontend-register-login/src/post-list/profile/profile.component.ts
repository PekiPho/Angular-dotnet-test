import { Component } from '@angular/core';
import { HistoryComponent } from "../history/history.component";

@Component({
  selector: 'profile',
  imports: [HistoryComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {

}
