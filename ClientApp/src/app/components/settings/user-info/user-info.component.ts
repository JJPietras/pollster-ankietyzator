
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
  //konwersja z stringa na tablice
  tags: string = "";
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
 

  constructor(public authenticationService: AuthenticationService,public httpclient: HttpClient, 
    @Inject('BASE_URL') baseUrl: string, private router: Router, public settingService: SettingsService) {
    
    this.baseUrl = baseUrl;

    this.tags =  this.authenticationService.user.value.tags.toString();
    this.keys = this.settingService.keys.value.find(result =>{
      if(this.authenticationService.user.value.eMail == result.eMail){
        return result;
      }     
    });
    
  }

  ngOnInit(){
    
    this.tagsArray = (this.tags.split('/')); 
    console.log("klucz: " + this.keys);
    
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

