import { Injectable, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient } from '@angular/common/http';

import Swal from 'sweetalert2';

@Injectable({
    providedIn: "root",
})

export class SettingsService {

    keysSource: BehaviorSubject<Key[]>;
    currentKeys: Observable<Key[]>;

    usersSource: BehaviorSubject<User[]>;
    currentUsers: Observable<User[]>;

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string){
      this.reloadUsers()
      this.reloadKeys()
    }

    reloadUsers(){
      this.http.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
        this.changeUsers(result.data);
        //console.log(this.users.value)
      }, error => {
        console.error("Failed to fetch the user session. Please, log in again.")
      });
    }

    reloadKeys(){
      this.http.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result => {
        this.changeKeys(result.data);
      }, error => {
        console.error("Failed to fetch the user session. Please, log in again.")
      });
    }

    get keys(){
      return this.keysSource;
    }

    changeKeys(keys: Key[]) {
      if (!this.keysSource) {
        this.keysSource = new BehaviorSubject(keys);
        this.currentKeys = this.keysSource.asObservable();
      }
      this.keysSource.next(keys);
    }

    changeUsers(users: User[]) {
      if (!this.usersSource) {
        this.usersSource = new BehaviorSubject(users);
        this.currentUsers = this.usersSource.asObservable();
      }
      this.usersSource.next(users);
    }

    get users(){
      return this.usersSource;
    }

    showLoading(message: string) {

      let timerInterval;
      Swal.fire({
        title: message,
        timer: 1000,
        timerProgressBar: true,
        didOpen: () => {
          Swal.showLoading()
          timerInterval = setInterval(() => { }, 100)
        },
        willClose: () => {
          clearInterval(timerInterval)
        }
      }).then((result) => {
        if (result.dismiss === Swal.DismissReason.timer) {
        }
      })
    }

}
