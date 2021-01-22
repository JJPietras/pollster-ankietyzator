
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
  declarations: [ AppComponent]
})


export class UserInfoComponent implements OnInit {

  baseUrl : string;
  keys: Key;
  keysM: Key[];
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
    //let tmpusers = this.authenticationService.user.value;
    //this.userTemp = this.authenticationService.user.value;

    this.httpclient.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
      tmpusers = result.data;
      this.tags = tmpusers.map(user => {return user["tags"];});

    }, error => console.error(error))

    

    this.httpclient.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.userTemp = result.data;

    }, error => console.error(error))

    /*this.userTemp = authenticationService.user.subscribe(result =>{
      return result;
    });*/


    //this.tags =  tmpusers.tags;
    //this.tags =  this.authenticationService.user.value.tags;
    //console.log(this.tags);


    this.httpclient.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
      this.keysM = result.data;
      if(!(result.data == null || result.data == undefined || !result.data.length)){
        this.keys = this.keysM.find(result => this.userTemp.eMail === result.eMail);
    }
    }, error => console.error(error))

    //this.keysM = this.settingService.keys.value;
    
    //this.keys = this.keysM.find(result => this.userTemp.eMail === result.eMail);
    //this.keysM = this.settingService.keys.value;
    
  }

  ngOnInit(){
    
    this.tagsArray = (this.tags.split('/')); 

    //this.keysM = this.settingService.keys.value;

    //this.keys = this.keysM.find(result => this.userTemp.map(value =>{ return value["eMail"];}) == result.eMail);
    //this.keys = this.keysM.filter(result => {return this.userTemp.map(value =>{ return value["eMail"];}) == result.eMail;});
    //console.log(this.keys);
        //console.log("tak");
        //return result;
      //}  
    //)


     /*this.keys = this.settingService.keys.value.find(result =>{
      if(this.authenticationService.user.value.eMail == result.eMail){
        return result;
      }  
    })*/
   
    /*this.httpclient.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
        this.tags = result.data.value.tags;
    })

    this.httpclient.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
        this.keys = result.data.value;
    })*/
    
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
    console.log(this.tags);
    console.log("tagi updateDTO: " + this.updateDTO.Tags);
    this.updateDTO.EMail =this.authenticationService.user.value.eMail;
    this.updateDTO.Key = this.keys.key

    if(this.updateDTO){
      
        this.httpclient.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", this.updateDTO).subscribe(result =>{
        //console.log(result);
    
         }, (error) => console.log(error.message + " + Failed to fetch the user session. Please, log in again."));}
         location.reload();

  }
  

  
}

