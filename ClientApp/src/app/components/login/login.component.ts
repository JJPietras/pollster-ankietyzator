
import {Component} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';

import { AuthenticationService } from "../../services/authorisation.service";

@Component({
  selector: 'app-user-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent{


  userLogin = new UserLogin();
  isLoading = false;
  http: HttpClient;
  baseUrl: string;

  constructor(public authenticationService: AuthenticationService) {
  }

  

}
