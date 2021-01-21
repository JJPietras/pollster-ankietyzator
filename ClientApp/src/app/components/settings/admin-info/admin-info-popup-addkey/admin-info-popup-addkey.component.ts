import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { Key } from 'protractor';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { SettingsService } from 'src/app/services/settings.service';
import Swal from 'sweetalert2';
import { AdminInfoComponent } from '../admin-info.component';

@Component({
  selector: 'app-admin-info-popup-addkey',
  templateUrl: './admin-info-popup-addkey.component.html',
  styleUrls: ['./admin-info-popup-addkey.component.css']
})
export class AdminInfoPopupAddkeyComponent implements OnInit {

  receivedData;
  usersAccounts;

  keyArray;
  newKey = {
    key: "",
    eMail: "",
    userType: 0
  }

  accountControl= new FormControl();

  keyControl = new FormControl('', [Validators.required,Validators.minLength(3)]);
  
  filteredOptions: Observable<User[]>;
  selectedEmails: string;
  
  get key(){ return this.keyControl};
  
  constructor(public dialogRef: MatDialogRef<AdminInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,  public httpclient: HttpClient, 
    public authenticationService: AuthenticationService,
    @Inject('BASE_URL') public baseUrl: string, private router: Router,
    public settingsService: SettingsService){

      this.receivedData = data;
      this.usersAccounts = this.authenticationService.users.value;
      this.keyArray = this.settingsService.keys.value;
      //this.receivedUsers = this.authenticationService.users.value;
    }

  ngOnInit(): void {
    this.filteredOptions = this.accountControl.valueChanges.pipe(
      startWith(''), 
      map(value => this._filter(value))
    )

  }

  onClose() {
    this.dialogRef.close();
  }

 

  _filter(value: string): User[] {
    
    const filterValue = value;
    return this.usersAccounts.filter(option =>
      option.eMail.includes(filterValue))
  }

  
  addKey(){
    let keyNameExists = this.keyArray.some(key => key.key === this.newKey.key);
    let keyEmailExists = this.keyArray.some(key => key.eMail === this.newKey.eMail);

    if(keyNameExists){
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Nazwa klucza już istnieje!'
      })
    }


    if(this.newKey.key.length == 0){
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Nie podałeś nazwy klucza'
      })
    }
  

    else if(keyEmailExists){
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Email jest przypisany do klucza'
      })
    }

    else if(this.newKey.userType == undefined){
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'Nie wybrałeś rodzaju użytkownika'
      })
    }

    else{
      this.settingsService.addKey(this.newKey);

      Swal.fire({
        title: 'Dodano nowy klucz !',
        confirmButtonText: `Ok`,
      }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        //if (result.isConfirmed) {
          location.reload();
       // } else if (result.isDenied) {
         // Swal.fire('Changes are not saved', '', 'info')
       // }
      })
      
    }

    
  }

   

}
