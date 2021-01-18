import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UpdateAccountDto } from "../models/updateDTO.model";
@Injectable({
    providedIn: "root",
})

export class SettingsService {

    //AccountSource: BehaviorSubject<Account>;
    currentAccount: Observable<Account>;

    private keySource: BehaviorSubject<Key>;
    private keysSource: BehaviorSubject<Key[]>;
    Keys: Observable<Key[]>;
    Key: Observable<Key>;
 

    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string){
        this.getKeys();
    }



    get key() {
      return this.keySource;
    }
  
    get keys(){
      return this.keysSource;
    }


    //GET KEYS
    public getKeys(){
      this.http.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
        this.keysSource = new BehaviorSubject(result.data);
        this.Keys = this.keys.asObservable();
      }, error => console.error("Failed to get users. Only Admin can get users accounts."))
     
    }

    //DELETE KEY
    public deleteKey(val : any){
      this.http.delete<UpdateAccountDto>(this.baseUrl + 'keys/remove-key/' + val).subscribe(result =>{
        console.log(result);

      }, error => console.error("Bład z usuniecie klucza"));
    }

    //UPDATE KEY
    public updateKey(val: any){
      this.http.put<UpdateAccountDto>(this.baseUrl + 'keys/update-key', val).subscribe(result =>{
        console.log(result);

      }, error => console.error("Bład z aktualizacji klucza"));
    }

    //ADD KEY
    public addKey(val: any){
      this.http.post<Key>(this.baseUrl + 'keys/add-key', val).subscribe(result =>{
        console.log(result);

      }, error => console.error("Problem z dodaniem klucza"));
    }
   


    public updateMyTags(val : any){
      this.http.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", val).subscribe(result =>{
        console.log(result);

      }, error => console.error("Failed to fetch the user session. Please, log in again."));

    }

    public updateOtherAccount(val: any) {
      this.http.put<UpdateAccountDto>(this.baseUrl + 'accounts/update-other-account', val).subscribe(result => {
          console.log(result);
        //console.log(this.user)
      }, error => console.error("Failed to fetch the user session. Please, log in again."));
    }

  


    /*changeTags(tag: string) {
        if (!this.pollSource){
            //this.pollSource = new BehaviorSubject(tag);
            //this.currentPoll = this.pollSource.asObservable();
        }
       // this.pollSource.next(tag);
    }*/

    pollStatsSource: BehaviorSubject<PollStats>;
    currentPollStats: Observable<PollStats>;

    changePollStats(poll: PollStats) {
        if (!this.pollStatsSource){
            this.pollStatsSource = new BehaviorSubject(poll);
            this.currentPollStats = this.pollStatsSource.asObservable();
        }
        this.pollStatsSource.next(poll);
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
  /*const httpOptions = {
    headers: new HttpHeaders({
       'Content-Type': 'application/xml,application/xhtml+xml,text/html'
    })
  };*/




}
