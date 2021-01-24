import { Component} from '@angular/core';
import { AuthenticationService } from "../../services/authorisation.service";
import { SettingsService } from 'src/app/services/settings.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent {
  constructor(public authenticationService: AuthenticationService){
  }
}
