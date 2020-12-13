import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient } from '@angular/common/http';
import { AngularFireAuth } from '@angular/fire/auth';
import { auth } from 'firebase/app';

@Injectable({
  providedIn: "root",
})
export class AuthenticationService{
  private windowHandle: Window;

  private userSource:BehaviorSubject<User>;
  currentUser:Observable<User>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getUser()
  }

  get user() {
    return this.userSource;
  }

  public getUser() {
    this.http.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.userSource = new BehaviorSubject(result.data);
      this.currentUser = this.user.asObservable();
      //console.log(this.user)
    }, error => console.error("Failed to fetch the user session. Please, log in again."));
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
