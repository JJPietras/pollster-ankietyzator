
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, NavigationExtras, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from "../../services/authorisation.service";

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  constructor(public authenticationService: AuthenticationService){

  }
}
