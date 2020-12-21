import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { FormsModule } from '@angular/forms';
import {  BrowserModule } from '@angular/platform-browser';
import { AppComponent } from '../../app.component';
import { AuthenticationService } from "../../../services/authorisation.service";
import { BehaviorSubject, Observable } from 'rxjs';
import { MatTableDataSource } from '@angular/material';

@Component({
  selector: 'app-admin-info',
  templateUrl: './admin-info.component.html',
  styleUrls: ['./admin-info.component.css']
})
export class AdminInfoComponent implements OnInit {

  //private userSource:BehaviorSubject<User>;
  //currentUsers:Observable<User>;
  rows: Number = 1; 
  namesAccounts: string[] = [];
  UsersAccounts: User[];
  filteredUsersAccounts: User[];
  currentModifyUser: Number = -1;
  oneUser: User ;
  objectProperty: Object[];
  objectCurrentProperty: Object;
  contentProperty: string = "";
  popup: boolean;

  dataSource;

 // Users: Observable<User[]>;
  //private usersSource: BehaviorSubject<User[]>;
  

 // constructor() { }

  constructor(public authenticationService: AuthenticationService, public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router) {

     this.UsersAccounts = this.authenticationService.users.value;
     console.log(this.authenticationService);
     this.dataSource = new MatTableDataSource(this.UsersAccounts);
     this.filteredUsersAccounts = this.UsersAccounts;
     
  }

  ngOnInit(){
    
  }

  /*get users(){
    return this.usersSource;
  }*/

  public onItemSelected(val: any){
    this.currentModifyUser  = Number(val);
    console.log(val);
    this.oneUser = this.UsersAccounts[String(this.currentModifyUser)];
    this.objectProperty = Object.keys(this.oneUser);
 
    //usuwanie z listy accountId oraz eMail
    this.objectProperty.forEach(value =>{
      if(value.toString() === "accountId" || value.toString() ==="eMail" )
        this.objectProperty.splice(this.objectProperty.indexOf(value),1);
    })
    
    //console.log(this.currentModifyUser);
  }


  public onItemSelectedProperty(val: any){


    console.log(this.objectCurrentProperty);
    this.objectCurrentProperty = this.objectProperty[val];
    //this.contentProperty = this.oneUser[val].value.toString();
    
    console.log(this.contentProperty);
    //console.log(this.currentModifyUser);
  }



  public filterEmail(val :any){
    //this.UsersAccounts = val.trim().toLowerCase();
    //this.dataSource.filter = val.trim().toLowerCase();
    this.filteredUsersAccounts = undefined;
    this.UsersAccounts.filter(user =>{
  
      if(user.eMail.includes(val) && (val.length != 0)){
     
        this.filteredUsersAccounts.push(user);
        
      }
    })

  
    if(val.length === 0 || val.trim() != ""){
      this.filteredUsersAccounts = this.UsersAccounts;
    }
 }



  public removeAccount(index){
    
    this.UsersAccounts.splice(index, 1);
    this.popup=false;
  }



}
