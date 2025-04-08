import { Component } from '@angular/core';
import { HistoryComponent } from "../history/history.component";

@Component({
  selector: 'profile',
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  public loadNumber:number=-1;


  changeNumber(value:number){
    this.loadNumber=value;
  }
}
