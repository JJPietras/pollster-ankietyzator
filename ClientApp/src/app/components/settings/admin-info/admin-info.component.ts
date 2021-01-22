import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { AuthenticationService } from "../../../services/authorisation.service";
import Swal from 'sweetalert2';
import { SettingsService } from 'src/app/services/settings.service';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { from, Observable } from 'rxjs';
import { AdminInfoPopupComponent } from './admin-info-popup-select/admin-info-popup.component';
import { map, startWith } from 'rxjs/operators';
import {MatDialog, MatDialogConfig} from '@angular/material';
import { AdminInfoPopupAddkeyComponent } from './admin-info-popup-addkey/admin-info-popup-addkey.component';


@Component({
  selector: 'app-admin-info',
  templateUrl: './admin-info.component.html',
  styleUrls: ['./admin-info.component.css']
})
export class AdminInfoComponent implements OnInit {

  

  emailFilter: string = "";
  rows: Number = 10; 

  namesAccounts: string[] = [];
  UsersAccounts: User[];
  filteredUsersAccounts: User[];
  objectCurrentProperty: Object;
  contentProperty: string = "";
  typeSelectedObject: Object;

  typeUserValueNumber: Number = -1;
  
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

  filteredOptions2: Observable<User[]>;

  
  
  updateAccount: UpdateAccountDto ={
    EMail: '',
    Tags: '',
    Key: ''

  };


  constructor(public authenticationService: AuthenticationService, public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router
    ,public settingsService: SettingsService, private dialog: MatDialog) {

     
     //this.keys = this.settingsService.keys.value;

     this.httpclient.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
       // this.keys = result.data;
        this.keysFiltered = result.data;
     })
     //this.keysFiltered = this.keys;
     //this.filteredUsersAccounts = this.UsersAccounts.filter(user =>user.userType != 2 )
     //this.UsersAccounts = this.authenticationService.users.value;

     this.httpclient.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
       this.UsersAccounts = result.data;
     })
     

     console.log(this.authenticationService.user.value);
     //console.log(this.authenticationService);
     //this.keysFiltered = this.keys;
     //this.keys = this.settingsService.keys.value;
    
     
  }

  ngOnInit(){
    
    this.newKey = {
      KeyId: undefined,
      key: "",
      eMail: "",
      userType: null
    }
    
/*
    this.filteredOptions = this.emailControl.valueChanges.pipe(
      startWith(''), 
      map(value => this._filter(value))
    )
*/
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


