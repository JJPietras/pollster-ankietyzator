import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UpdateAccountDto } from "../models/updateDTO.model";
import { catchError } from 'rxjs/operators';
import Swal from "sweetalert2";
import {Router, ActivatedRoute} from '@angular/router';
//import { AngularFireAuth } from '@angular/fire/auth';
//import { auth } from 'firebase/app';

@Injectable({
  providedIn: "root",
})
export class AuthenticationService{

  private userSource:BehaviorSubject<User>;
  currentUser:Observable<User>;
  Users: Observable<User[]>;

  constructor(private http: HttpClient, private router: Router, @Inject('BASE_URL') private baseUrl: string) {
    this.getUser()
  }

  public tryToGetSession(){
    this.getUser()
    if (!this.userSource)
      this.router.navigate(['/user-login']);
  }

  get user() {
    return this.userSource;
  }

  public getUser() {
    this.http.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.userSource = new BehaviorSubject(result.data);
      this.currentUser = this.user.asObservable();
    }, error => {
      console.error("Failed to fetch the user session. Please, log in again.")
      this.userSource = null;
    });
  }

  public SignInWithGoogle() {
    document.location.href = (this.baseUrl + 'google/google-login')
    
  }

  public LogOutFromGoogle() {
    document.location.href = (this.baseUrl + 'google/google-logout')
  }
  
}
