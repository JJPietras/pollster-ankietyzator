
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
import { Observable } from 'rxjs';
import { subscribeOn } from 'rxjs/operators';


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
  declarations: [ AppComponent]
})


export class UserInfoComponent implements OnInit {

  baseUrl : string;
  keys: Key;
  keysM: Key[];
  emailAccount: string = "";
  //konwersja z stringa na tablice
  tags: string = "";
  tagsArray: string[] = new Array<string>();
  //index tagu ktory zostal wybrany
   currentIndex: number;
  //modyfikowany
  modifiedtext: string;
  //
  keyChange: string;
  userTemp;
  
  updateDTO : UpdateAccountDto = {
    EMail: '',
    Tags: 'w',
    Key: ''
  };
 

  constructor(public authenticationService: AuthenticationService,public httpclient: HttpClient, 
    @Inject('BASE_URL') baseUrl: string, private router: Router, public settingService: SettingsService) {
    
    this.baseUrl = baseUrl;
    let tmpusers;
    this.emailAccount = this.authenticationService.user.value.eMail;
    //let tmpusers = this.authenticationService.user.value;
    //this.userTemp = this.authenticationService.user.value;

    this.httpclient.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
      tmpusers = result.data;
      this.tags = tmpusers

    }, error => console.error(error))

    

    this.httpclient.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.userTemp = result.data;
      
    }, error => console.error(error))

     
    
    this.tags =  this.authenticationService.user.value.tags;

    this.httpclient.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
      
      this.keysM = result.data;
      this.keys = this.settingService.findKey(this.authenticationService.user.value.eMail);
      
    }, error => console.error(error));
    // /this.keys = this.settingService.findKey(this.userTemp.eMail);
    
    
  }

  ngOnInit(){
    
    this.tagsArray = (this.tags.split('/')); 

  }

  public findKey(value: Key[], user: User){
    this.keys = value.find(result => user.eMail === result.eMail);
    //return this.keys;
  }

  //ADD TAG
  public addTag(){
    if(!(this.modifiedtext.length == 0)){
      this.tags = this.tags + '/'+ this.modifiedtext;
      this.tagsArray.push(this.modifiedtext);
      this.modifiedtext = "";

      this.tags ="";
      this.tags = this.tagsArray.join('/');
    }
     

    
    
  }

  //CHANGE TAG
  public changeTag(){
    if(!(this.modifiedtext.length == 0)){
      if (this.currentIndex !== -1) {
          this.tagsArray[this.currentIndex] = this.modifiedtext;
      }  
      this.modifiedtext = "";

      this.resetStringItems(this.tagsArray);
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

  //check if obj is empty
  isEmptyObject(obj) {
    return (obj && (Object.keys(obj).length === 0));
  }


  //save changes
  saveChanges(){

    this.updateDTO.Tags = this.tags;
    this.updateDTO.EMail =this.emailAccount;
    this.updateDTO.Key = this.keys.key

    if(this.updateDTO){
      
        this.httpclient.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", this.updateDTO).subscribe(result =>{
        //console.log(result);
        location.reload();
         }, (error) => console.log(error.message + " + Failed to save changes."));}
         

  }
  

  
}

