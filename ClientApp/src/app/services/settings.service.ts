import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UpdateAccountDto } from "../models/updateDTO.model";
import { map, filter, switchMap } from 'rxjs/operators';
import Swal from 'sweetalert2';

@Injectable({
    providedIn: "root",
})

export class SettingsService {

    keysSource: BehaviorSubject<Key[]>;
    currentKeys: Observable<Key[]>;

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string){
        
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
