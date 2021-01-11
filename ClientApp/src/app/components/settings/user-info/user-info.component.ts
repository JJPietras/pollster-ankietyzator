
import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { FormsModule } from '@angular/forms';
import {  BrowserModule } from '@angular/platform-browser';
import { AppComponent } from '../../app.component';
import { AuthenticationService } from "../../../services/authorisation.service";
import { UpdateAccountDto } from '../../../models/updateDTO.model';
import { throwToolbarMixedModesError } from '@angular/material';
import { SettingsService } from 'src/app/services/settings.service';


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

  //http : HttpClient;
  baseUrl : string;
  


  //konwersja z stringa na tablice
  tags: string = "to/nie/to/pwowinno/sie/wyswietlic";
  tagsArray: string[] = new Array<string>();
  //index tagu ktory zostal wybrany
   currentIndex: number;
  //modyfikowany
  modifiedtext: string;
  //
  keyChange: string;
  
  updateDTO : UpdateAccountDto = {
    EMail: '',
    Tags: 'w',
    Key: ''
  };
 

  
  
  
// pobranie uzytkownika i dodanie tagow


  constructor(public authenticationService: AuthenticationService,public httpclient: HttpClient, 
    @Inject('BASE_URL') baseUrl: string, private router: Router, public settingService: SettingsService) {
    //this.http = httpclient;
    this.baseUrl = baseUrl;
   
    this.tags =  this.authenticationService.user.value.tags.toString();
   
  }

  ngOnInit(){

    this.tags =  this.authenticationService.user.value.tags.toString();
    //console.log(this.items);
    console.log(this.authenticationService.user.value.tags);
    this.tagsArray = (this.tags.split('/')); 
    
    
  }



  //ADD TAG
  public addTag(){
    
     this.tags = this.tags + '/'+ this.modifiedtext;
     this.tagsArray.push(this.modifiedtext);
     this.modifiedtext = "";

    this.tags ="";
     this.tagsArray.forEach(slowo => {
        this.tags = this.tags +  "/" + slowo.toString(); 
     })
    
  }

  //CHANGE TAG
  public changeTag(){
    
    /*if(this.modifiedtext.length === 0){
      return 
    }*/

    if (this.currentIndex !== -1) {
        this.tagsArray[this.currentIndex] = this.modifiedtext;
    }  
    this.modifiedtext = "";

    this.resetStringItems(this.tagsArray);
      
    this.updateDTO.Tags = this.tags;
    this.updateDTO.EMail =this.authenticationService.user.value.eMail;
    this.updateDTO.Key = "klucz1"
 
    console.log(this.updateDTO.Tags);

 
    if(this.updateDTO){
        this.httpclient.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", this.updateDTO).subscribe(result =>{
        console.log(result);
    
    }, (error) => console.log(error.message + " + Failed to fetch the user session. Please, log in again."));

    }

  }




  //REMOVE TAG
  public removeTag(){
      
   
    if (this.currentIndex !== -1) {
        this.tagsArray.splice(this.currentIndex, 1);
    }  
    this.modifiedtext = "";

    this.resetStringItems(this.tagsArray);
    
 }

 //SELECTED TAG FROM LIST 
   onItemSelected(val: any){
    
    this.modifiedtext = String(val);
    this.currentIndex  = this.tagsArray.indexOf(this.modifiedtext);
    
  }




  //METHOD TO USE RESET STRING
  public resetStringItems(val: string[]){

    this.tags ="";
    val.forEach(slowo => {
        this.tags = this.tags + slowo.toString() + "/" ; 
     })
     this.tags = this.tags.slice(0,-1);

  }

  //SEND KEY
  public sendKey(){
    console.log(this.keyChange);

    if(this.keyChange.length < 4){
      console.log('Podany klucz jest za krótki')
    }
    this.keyChange = '';
    //this.httpclient.put(this.baseUrl +  , this.keyChange);
  }


}

