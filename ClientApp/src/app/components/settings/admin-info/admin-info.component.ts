import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { AuthenticationService } from "../../../services/authorisation.service";
import { MatIcon, MatTableDataSource } from '@angular/material';
import Swal from 'sweetalert2';
import { SettingsService } from 'src/app/services/settings.service';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { from, Observable } from 'rxjs';
import { AdminInfoPopupComponent } from './admin-info-popup-select/admin-info-popup.component';
import { map, startWith } from 'rxjs/operators';
import {MatDialog, MatDialogConfig} from '@angular/material';
import { AdminInfoPopupAddkeyComponent } from './admin-info-popup-addkey/admin-info-popup-addkey.component';
//import { generateKeyPairSync } from 'crypto';

@Component({
  selector: 'app-admin-info',
  templateUrl: './admin-info.component.html',
  styleUrls: ['./admin-info.component.css']
})
export class AdminInfoComponent implements OnInit {

  
  rows: Number = 10; 
  namesAccounts: string[] = [];
  UsersAccounts: User[];
  filteredUsersAccounts: User[];
  currentModifyUser: Number = -1;
  oneUser: User ;
  objectProperty: Object[];
  objectCurrentProperty: Object;
  contentProperty: string = "";
  typeSelectedObject: Object;

  typeUserValueNumber: Number = -1;
  emailFilter: string = "";


  keys: Key[];
  selectedEmails: string;
  keyName: string ="";
  keyUserType: number;
  userTags: string[];
  newKey: Key;
  keysFiltered: Key[];

  keyForm = new FormGroup({
    name: new FormControl('',Validators.required)
  })

  get key(){return this.keyForm.get('name')};



  
  emailControl = new FormControl();
  accountControl = new FormControl();

  filteredOptions: Observable<Key[]>;
  filteredOptions2: Observable<User[]>;

  
  
  updateAccount: UpdateAccountDto ={
    EMail: '',
    Tags: '',
    Key: ''

  };

  objectProperty2: any[];


  

  constructor(public authenticationService: AuthenticationService, public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router
    ,public settingsService: SettingsService, private dialog: MatDialog) {

     this.UsersAccounts = this.authenticationService.users.value;
     console.log(this.authenticationService);
     this.keys = this.settingsService.keys.value;
     this.keysFiltered = this.keys;
     console.log(this.keys);
     //this.dataSource = new MatTableDataSource(this.UsersAccounts);
     this.filteredUsersAccounts = this.UsersAccounts.filter(user =>user.userType != 2 )
     
  }

  ngOnInit(){
    this.newKey = {
      KeyId: undefined,
      key: "",
      eMail: "",
      userType: null
    }
    

    this.filteredOptions = this.emailControl.valueChanges.pipe(
      startWith(''), 
      map(value => this._filter(value))
    )

    this.filteredOptions2 = this.accountControl.valueChanges.pipe(
      startWith(''), 
      map(value => this._filter2(value))
    )


  }


  public onCreate(key: Key){
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = false;
      dialogConfig.autoFocus = true;
      dialogConfig.width = "60%";
      dialogConfig.data = key;
      this.dialog.open(AdminInfoPopupComponent, dialogConfig).afterClosed().subscribe(result =>{
       // this.showData
      });
  }

  public onAddKey(){
    const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = false;
      dialogConfig.autoFocus = true;
      dialogConfig.width = "60%";
      dialogConfig.data = this.filteredOptions2;
      this.dialog.open(AdminInfoPopupAddkeyComponent, dialogConfig).afterClosed().subscribe(result =>{
       // this.showData
      });
  }

  private _filter(value: string): Key[] {
    //const filterValue = value.eMail;
    const filterValue = value;
    return this.keys.filter(option =>
      option.eMail.includes(filterValue))
      //option.eMail.includes(filterValue));
  }

  t
  private _filter2(value: string): User[] {
    //const filterValue = value.eMail;
    const filterValue = value;
    return this.UsersAccounts.filter(option =>
      option.eMail.includes(filterValue))
      //option.eMail.includes(filterValue));
  }


  displayFn(subject){
    return subject ? subject.name : undefined;
  }


 
  public onItemSelected(val: any){

    this.currentModifyUser  = Number(val);
    this.oneUser = val;
    this.objectProperty = [];

    this.objectProperty = Object.keys(this.oneUser);
    this.objectProperty2 = Object.values(this.oneUser);
    
    this.objectProperty.forEach(value =>{
      if(value.toString() === "accountId" || value.toString() ==="eMail" || value.toString() === "name"){
        this.objectProperty2.splice(this.objectProperty.indexOf(value),1);
        this.objectProperty.splice(this.objectProperty.indexOf(value),1);
        
      }
      this.contentProperty = "";
      //console.log(this.contentProperty);
    })
  }


  tag: string ='';
  public getTag(value: any){
    console.log("ta: " + value.split('/',value));
    this.tag = this.oneUser.tags.split('/')[value];
    
  }

  public onItemSelectedProperty(val: any){
    this.typeSelectedObject = this.objectProperty[val];
    this.objectCurrentProperty = this.objectProperty2[val];
    this.contentProperty = this.objectProperty2[val];
    
    console.log("tu prpoperty: " + this.objectProperty2[val] + "oraz " + this.typeSelectedObject);
   
  }

  public changeProperty(){
      if(this.contentProperty != " " && this.contentProperty.length != 0){
          console.log(this.oneUser.eMail);

          //this.updateAccount.EMail = this.oneUser.eMail;
          //this.updateAccount.Tags = this.contentProperty;
          //this.updateAccount.Key = "w"; 
         // this.settingsService.updateOtherAccount(this.updateAccount);
      }
  }

  public updateOtherAccount(val :any){
    console.log("tu niby val: " + this.contentProperty);

    this.updateAccount.EMail = this.oneUser.eMail;
    this.updateAccount.Tags = this.contentProperty;
    this.updateAccount.Key = "klucz1";

    this.httpclient.put<UpdateAccountDto>(this.baseUrl + "accounts/update-other-account", this.updateAccount).subscribe(result =>{
      console.log(result);
    }, error => console.log(error));
  }

  //KLUCZE
  public removeKey(val:any){
    let timerInterval;

    Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć klucz ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          this.settingsService.deleteKey(val);
          //this.UsersAccounts.splice(this.UsersAccounts.indexOf(val),1);
          //this.filteredUsersAccounts = this.UsersAccounts;
          //Swal.fire('Usunięto');
          
            Swal.fire({
              title: 'Usunięto',
              timer: 800,
              timerProgressBar: true,
              didOpen: () => {
                Swal.showLoading()
                timerInterval = setInterval(()=>{}, 100) 
              
              },
              willClose: () => {
                clearInterval(timerInterval)
              } 
            }).then((result) => {
              location.reload();
              if (result.dismiss === Swal.DismissReason.timer) {
                console.log('I was closed by the timer')
              }
            })
        } 
      }
    );
  }


  public addKey(){
     
    this.newKey.key = this.keyName; 
    console.log(this.newKey.key);
    this.newKey.eMail = this.selectedEmails;
    console.log(this.newKey.eMail);
    this.newKey.userType = this.keyUserType;
    
    let keyNameExists = this.keys.some(key => key.key === this.keyName);
    let keyEmailExists = this.keys.some(key => key.eMail === this.selectedEmails);

    if(keyNameExists){
        console.log("nazwa klucza juz istnieje.");
    }

    if(keyEmailExists){
      console.log("klucz z wybranym email'em juz istnieje.");
    }

    this.settingsService.addKey(this.newKey);
    this.keys = this.settingsService.keys.value;
  }

  public updateKey(val:any){
    this.settingsService.updateKey(val);
    
  }

  public keySelected(val : any){
  
     this.UsersAccounts.map(user =>{
      if(val == user.eMail)
          this.userTags =  user.tags.split('/');
    });

    console.log(this.userTags);

    var tmp = this.keys.find(key => {key.eMail == val});
    console.log(val);
    console.log("Email: " + tmp.eMail.toString());
    
    this.selectedEmails = tmp.eMail;
    this.keyName = tmp.key;
    this.keyUserType = tmp.userType;

  }

  public filterEmail(){
    this.filterTypeUser(this.typeUserValueNumber);
  }

 public filterTypeUser(val: any){

   this.typeUserValueNumber = val;
  //this.filteredUsersAccounts = this.UsersAccounts.filter(user =>user.userType != 2 && user.eMail.includes(this.emailFilter));
  this.keysFiltered = this.keys.filter(key => key.eMail.includes(this.emailFilter));
  console.log(val);
  if(val != -1){
    this.keysFiltered = this.keysFiltered.filter(key => key.userType == val && key.eMail.includes(this.emailFilter));
  }
 }



}





/*
  public filterEmail(){
    this.filterTypeUser(this.typeUserValueNumber);
  }

 public filterTypeUser(val: any){

   this.typeUserValueNumber = val;
  //this.filteredUsersAccounts = this.UsersAccounts.filter(user =>user.userType != 2 && user.eMail.includes(this.emailFilter));
  this.filteredUsersAccounts = this.UsersAccounts.filter(user => user.eMail.includes(this.emailFilter));
  console.log(val);
  if(val != -1){
   this.filteredUsersAccounts = this.filteredUsersAccounts.filter(user => user.userType == val && user.eMail.includes(this.emailFilter));
  }
 }*/

 /*
  public removeAccount(val:any){
    let timerInterval;

    Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć konto ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          this.UsersAccounts.splice(this.UsersAccounts.indexOf(val),1);
          this.filteredUsersAccounts = this.UsersAccounts;
          //Swal.fire('Usunięto');
          
            Swal.fire({
              title: 'Usunięto',
              timer: 800,
              timerProgressBar: true,
              didOpen: () => {
                Swal.showLoading()
                timerInterval = setInterval(()=>{}, 100) 
              
              },
              willClose: () => {
                clearInterval(timerInterval)
              }
            }).then((result) => {
              if (result.dismiss === Swal.DismissReason.timer) {
                console.log('I was closed by the timer')
              }
            })
        } 
      }
    );
  }
*/


