import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UpdateAccountDto } from "../models/updateDTO.model";
import { catchError } from 'rxjs/operators';
import Swal from "sweetalert2";

//import { AngularFireAuth } from '@angular/fire/auth';
//import { auth } from 'firebase/app';

@Injectable({
  providedIn: "root",
})
export class AuthenticationService{ś
  private windowHandle: Window;
  private userSource:BehaviorSubject<User>;
  private usersSource: BehaviorSubject<User[]>;
  currentUser:Observable<User>;
  Users: Observable<User[]>;
  //###########################
  //do pytan ##################
  //private questionsSource: BehaviorSubject<Question[]>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getUser()
    this.getUsers();

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
    }, error => console.error("Failed to fetch the user session. Please, log in again."));
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

  /*const httpOptions = {
    headers: new HttpHeaders({
       'Content-Type': 'application/xml,application/xhtml+xml,text/html'
    })
  };*/

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
    //return this.http.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", val) //ze niby teraz 400 i nie moze rozpoznac zadania polcenia - 400 http
    //return this.http.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", val) //wykrywa ze null i nie obsluguje czegps innego - 415 http
          //.post<Request>("https://localhost:5001/settings/accounts/update-my-account", val)
          
          
  }

/*
  this.http
          .post<OrderRequest>(AppSettings.API_ENDPOINT + "orders", body)
          .subscribe(
            (result) => {
              console.log(result);
              Swal.fire("Gratulacje", "Dokonano zakupu pomyślnie", "info").then(
                () => {
                  window.location.reload();
                }
              );
            },
            (error) => {
              Swal.fire("Błąd", error.message, "error");
            }
          );*/

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
