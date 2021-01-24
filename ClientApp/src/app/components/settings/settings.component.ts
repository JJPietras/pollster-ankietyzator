
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, OnDestroy, ViewChild, Inject } from '@angular/core';
import { Router, NavigationExtras, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from "../../services/authorisation.service";


@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {

  userType: any;

  constructor(public authenticationService: AuthenticationService, public http: HttpClient,
    @Inject('BASE_URL') baseUrl: string){

      let user;

    this.http.get<Request>(baseUrl + 'accounts/get-account').subscribe(result => {
       user = result.data;
      if(user){
        this.userType = user.userType;
        //this.userType = user.map(u => {return u["userType"];});
      }
      
    }, error => console.log(error))
  }
}
