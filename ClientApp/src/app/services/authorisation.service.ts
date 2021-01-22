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
  private windowHandle: Window;
  private userSource:BehaviorSubject<User>;
  private usersSource: BehaviorSubject<User[]>;
  currentUser:Observable<User>;
  Users: Observable<User[]>;
  //###########################
  //do pytan ##################
  //private questionsSource: BehaviorSubject<Question[]>;

  constructor(private http: HttpClient, private router: Router, @Inject('BASE_URL') private baseUrl: string) {
    this.getUser()
    this.getUsers();

  }

  public tryToGetSession(){
    this.getUser()
    if (!this.userSource)
      this.router.navigate(['/user-login']);
  }

  get user() {
    return this.userSource;
  }

  get users(){
    return this.usersSource;
  }

  public getUser() {
    this.http.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.userSource = new BehaviorSubject(result.data);
      this.currentUser = this.user.asObservable();
      //console.log(this.user)
    }, error => {
      console.error("Failed to fetch the user session. Please, log in again.")
      this.userSource = null;
    });
  }

  /*
  //Admin
  public getUsers(){
    this.http.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result =>{
      this.usersSource = new BehaviorSubject(result.data);
      this.Users = this.users.asObservable();
    }, error => console.error("Failed to get users. Only Admin can get users accounts."))
  }*/


  //Admin
  public getUsers(){
    this.http.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result =>{
      this.usersSource = new BehaviorSubject(result.data);
      this.Users = this.users.asObservable();
    }, error => console.error("Failed to get users. Only Admin can get users accounts."))
   
  }


   
  public updateUser(val: UpdateAccountDto) {
    console.log(val);
    //return this.http.put(this.baseUrl + 'accounts/update-my-account', val); "keys/update-key"
     this.http.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", null).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {
        Swal.fire("Błąd", error.message, "error");
      })
       
  }

  public updateOtherUser(val: UpdateAccountDto) {
    console.log(val);
     this.http.put<UpdateAccountDto>(this.baseUrl + "update-other-account", val).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {
        Swal.fire("Błąd", error.message, "error");
      })
       
  }


  public SignInWithGoogle() {
    document.location.href = (this.baseUrl + 'google/google-login')
    
  }

  public LogOutFromGoogle() {
    document.location.href = (this.baseUrl + 'google/google-logout')
  }


  //Experimental
  loginViaGoogle(){
    this.windowHandle = this.createOauthWindow(this.baseUrl + 'google/google-login', 'Zaloguj przez Google');
  }

  private createOauthWindow(url: string, name = 'Authorization', width = 500, height = 600, left = 0, top = 0) {
    if (url == null) {
        return null;
    }
    const options = `width=${width},height=${height},left=${left},top=${top}`;
    return window.open(url, name, options);
  }

  


}
