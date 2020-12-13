
import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { FormsModule } from '@angular/forms';
import {  BrowserModule } from '@angular/platform-browser';
import { AppComponent } from '../../app.component';
import { AuthenticationService } from "../../../services/authorisation.service";
//import { UpdateAccountDto } from '../../../models/updateDTO.model';


@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})

@NgModule({
  imports: [
    BrowserModule,
    FormsModule
  ],
  declarations: [
    AppComponent

  ]
})


export class UserInfoComponent implements OnInit {

  http : HttpClient;
  baseUrl : string;
  


  //konwersja z stringa na tablice
  items: string = "to/nie/to/pwowinno/sie/wyswietlic";
  slowka: string[] = new Array<string>();
  //index tagu ktory zostal wybrany
   currentIndex: number;
  //modyfikowany
  modifiedtext: string;
  //
  keyChange: string;
  
  //updateDTO : UpdateAccountDto;
 
  
  
// pobranie uzytkownika i dodanie tagow


  constructor(public authenticationService: AuthenticationService, httpclient: HttpClient, @Inject('BASE_URL') baseUrl: string, private router: Router ) {
    this.http = httpclient;
    this.baseUrl = baseUrl;
   
    this.items =  this.authenticationService.user.value.tags.toString();
   
  }

  ngOnInit(){

    this.items =  this.authenticationService.user.value.tags.toString();
    //console.log(this.items);
    console.log(this.authenticationService.user.value.tags);
    this.slowka = (this.items.split('/')); 
    
  }



  //ADD TAG
  public addTag(){
    
     this.items = this.items + '/'+ this.modifiedtext;
     this.slowka.push(this.modifiedtext);
     this.modifiedtext = "";

    this.items ="";
     this.slowka.forEach(slowo => {
        this.items = this.items +  "/" + slowo.toString(); 
     })
    
  }

  //CHANGE TAG
  public changeTag(){
    
    if (this.currentIndex !== -1) {
        this.slowka[this.currentIndex] = this.modifiedtext;
    }  
    this.modifiedtext = "";

    this.resetStringItems(this.slowka);
      
    this.authenticationService.user.value.tags = this.items;
    this.http.put<User>(this.baseUrl + "accounts/update-my-account", this.authenticationService.user);
    this.authenticationService.getUser();
  }




  //REMOVE TAG
  public removeTag(){
      
   
    if (this.currentIndex !== -1) {
        this.slowka.splice(this.currentIndex, 1);
    }  
    this.modifiedtext = "";

    this.resetStringItems(this.slowka);
    
 }

 //SELECTED TAG FROM LIST 
   onItemSelected(val: any){
    
    this.modifiedtext = String(val);
    this.currentIndex  = this.slowka.indexOf(this.modifiedtext);
    
  }




  //METHOD TO USE RESET STRING
  public resetStringItems(val: string[]){

    this.items ="";
    val.forEach(slowo => {
        this.items = this.items + slowo.toString() + "/" ; 
     })
     this.items = this.items.slice(0,-1);

  }

  //SEND KEY
  public sendKey(){
    console.log(this.keyChange);
  }


}

