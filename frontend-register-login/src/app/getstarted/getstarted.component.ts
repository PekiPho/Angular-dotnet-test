import { Component } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';
import { NavigationBarComponent } from "../../start/navigation-bar/navigation-bar.component";

@Component({
  selector: 'getstarted',
  imports: [RouterLink, RouterModule, NavigationBarComponent,NavigationBarComponent],
  templateUrl: './getstarted.component.html',
  styleUrl: './getstarted.component.scss'
})
export class GetstartedComponent {

}
