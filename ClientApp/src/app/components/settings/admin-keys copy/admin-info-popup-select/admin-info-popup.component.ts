import { CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { SettingsService } from 'src/app/services/settings.service';
import Swal from 'sweetalert2';
import { AdminKeysComponent } from '../admin-keys.component';

@Component({
  selector: 'app-admin-info-popup',
  templateUrl: './admin-info-popup.component.html',
  styleUrls: ['./admin-info-popup.component.css']
})


export class AdminInfoPopupComponent implements OnInit {

  receivedData;
  receivedUsers;
  receivedUserTags;
  receivedUser;
  currentUserTag = "";
  numberUserTag;

  modifiedtext: string;
  currentIndex: number;
   tagsArray: string[] = new Array<string>();
   tags: string = "";


  constructor(public dialogRef: MatDialogRef<AdminKeysComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,  public httpclient: HttpClient, 
    public authenticationService: AuthenticationService,
    @Inject('BASE_URL') public baseUrl: string, private router: Router,
    public settingsService: SettingsService){

      this.receivedData = data;

      /*this.httpclient.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
        this.receivedUsers = result.data;
      }, error => console.log(error))*/
      this.receivedUsers = this.authenticationService.users.value;
      console.log("http: " + this.httpclient);


      //this.selectUser();
      
    
    } 
  


  ngOnInit() {
    //this.service.getEmployees();
    console.log("basuURL: " + this.baseUrl + " " + this.httpclient);
    //const user = this.receivedUsers.find(element => element.eMail === this.receivedData.eMail);
    /*this.receivedUser = this.receivedUsers.find(element => element.eMail === this.receivedData.eMail);
    this.tags = this.receivedUser.tags;
    this.tagsArray = (this.tags.split('/'));*/
    //this.tagsArray = (this.tags.split('/')); 
   // this.receivedUser = user;
    this.selectUser();
  }

  onSave(){

    this.httpclient.put<UpdateAccountDto>(this.baseUrl + 'keys/update-key', this.receivedData).subscribe(result =>{

    }, error => console.log(error))
    //this.settingsService.updateKey(this.receivedData);

  }

   selectUser(){

    //const user = this.receivedUsers.find(element => element.eMail === this.receivedData.eMail);
    this.receivedUser = this.receivedUsers.find(element => element.eMail === this.receivedData.eMail);
    this.tags = this.receivedUser.tags;
    this.tagsArray = (this.tags.split('/')); 
    //this.receivedUser = user;
    //console.log(user.name);
    //console.log(user.tags);
    //return user;
  }

  onChangeUserProperties(){
    this.onSave();

    let updateAccount: UpdateAccountDto = {EMail:"", Key:"", Tags:""};
    updateAccount.EMail = this.receivedData.eMail;
    updateAccount.Key = this.receivedData.key;
    updateAccount.Tags = this.tags;
    
    
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

        Swal.fire({
          title: 'Dokonano zmiany !',
          confirmButtonText: `Ok`,
        }).then((result) => {
            location.reload();
        })
       
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



  onItemSelected(val: any){
    
    this.modifiedtext = String(val);
    this.currentIndex  = this.tagsArray.indexOf(this.modifiedtext);
    
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


 public resetStringItems(val: string[]){

  this.tags ="";
  val.forEach(slowo => {
      this.tags = this.tags + slowo.toString() + "/" ; 
   })
   this.tags = this.tags.slice(0,-1);

}




}