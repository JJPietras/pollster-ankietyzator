
import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';

@Component({
  selector: 'app-user-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent{

  userLogin = new UserLogin();
  isLoading = false;


}
