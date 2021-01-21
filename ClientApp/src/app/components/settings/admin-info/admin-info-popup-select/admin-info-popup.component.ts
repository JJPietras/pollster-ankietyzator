import { CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { SettingsService } from 'src/app/services/settings.service';
import Swal from 'sweetalert2';
import { AdminInfoComponent } from '../admin-info.component';

@Component({
  selector: 'app-admin-info-popup',
  templateUrl: './admin-info-popup.component.html',
  styleUrls: ['./admin-info-popup.component.css']
})


export class AdminInfoPopupComponent implements OnInit {

  receivedData;
  receivedUsers;
  receivedUserTags;
  receviedUser;
  currentUserTag = "";
  numberUserTag;



  constructor(public dialogRef: MatDialogRef<AdminInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,  public httpclient: HttpClient, 
    public authenticationService: AuthenticationService,
    @Inject('BASE_URL') public baseUrl: string, private router: Router,
    public settingsService: SettingsService){

      this.receivedData = data;
      this.receivedUsers = this.authenticationService.users.value;
    } 
  



  ngOnInit() {
    //this.service.getEmployees();
    this.selectUser();
  }

  onSave(){
    this.settingsService.updateKey(this.receivedData);

  }

   selectUser(){

    const user = this.receivedUsers.find(element => element.eMail == this.receivedData.eMail);
    this.receivedUserTags = user.tags;
    this.receviedUser = user;
    console.log(user.name);
    console.log(user.tags);
    //return user;
  }

  onChangeUserProperties(){
    this.onSave();

    let updateAccount: UpdateAccountDto = {EMail:"", Key:"", Tags:""};
    updateAccount.EMail = this.receivedData.eMail;
    updateAccount.Key = this.receivedData.key;
    updateAccount.Tags = this.receivedUserTags;

    if(this.receivedData.key.length == 0)
    {
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Pole jest puste. Wprowadź klucz.'
      })
    }
    else
    {
      this.httpclient.put<UpdateAccountDto>(this.baseUrl + "accounts/update-other-account",  updateAccount).subscribe(result =>{
        console.log(result);
      }, (error) => console.log(error.message + " + Failed."));
    }
  
  }



  selectedTag(value: any){
    this.currentUserTag = value;
    this.numberUserTag = this.receivedUserTags.split('/').indexOf(this.currentUserTag);
  }

  userTag(){
    let stringArray: string[];
    stringArray = this.receivedUserTags.split('/');
    if(this.currentUserTag == "" || this.currentUserTag.length ==0){
      stringArray.splice(this.numberUserTag, 1);
      this.numberUserTag = 0;
    }else{
      stringArray[this.numberUserTag] = this.currentUserTag; 
    }
      this.receivedUserTags = stringArray.join('/');
    
    
  }

  onClose() {
    //this.service.form.reset();
    //this.service.initializeFormGroup();
    this.dialogRef.close();
  }

}