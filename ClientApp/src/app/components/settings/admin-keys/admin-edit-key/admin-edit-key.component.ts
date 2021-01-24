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
  templateUrl: './admin-edit-key.component.html'
})

export class AdminEditKeyComponent  {

  key: Key;
  editMode = false;
  currentIndex: number;
   tagsArray: string[] = new Array<string>();
   tags: string = "";

  constructor(public dialogRef: MatDialogRef<AdminKeysComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Key,  public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, public settingsService: SettingsService){
      if (data){
        this.key = data;
        this.editMode = true;
      }
      else{
        this.key = {
          key: "",
          eMail: "",
          userType: 0
        }
        this.editMode = false;
      }
    } 
  
  onSave(){
    if (this.validateKey()){
      this.settingsService.showLoading("Zapisywanie zmian.");
      this.httpclient.put<Key>(this.baseUrl + 'keys/update-key', this.key).subscribe(result =>{
        Swal.close();
        this.dialogRef.close();
      }, (error) => {
        Swal.close();
        Swal.fire("Błąd", error.message, "error");
        console.log(error.message);
      })
    }
  }

  onClose() {
    this.dialogRef.close();
  }

  validateKey(): boolean{
    let keyNameExists = this.settingsService.keys.value.some(key => key.key === this.key.key);
    let keyEmailExists = this.settingsService.keys.value.some(key => key.eMail === this.key.eMail);
    
    if(keyNameExists){
      Swal.fire({
        icon: 'error',
        title: 'Błąd',
        text: 'Nazwa klucza już istnieje!'
      })
    }

    if(this.key.key.length == 0){
      Swal.fire({
        icon: 'error',
        title: 'Błąd',
        text: 'Nie podałeś nazwy klucza'
      })
    }
  
    else if(keyEmailExists){
      Swal.fire({
        icon: 'error',
        title: 'Błąd',
        text: 'Email jest przypisany do klucza'
      })
    }

    else if(this.key.userType == undefined){
      Swal.fire({
        icon: 'error',
        title: 'Błąd',
        text: 'Nie wybrałeś rodzaju użytkownika'
      })
      return false;
    }
  return true;
  }

  addKey(){
      if (this.validateKey()){
        this.httpclient.post<Key>(this.baseUrl + 'keys/add-key', this.key).subscribe(result =>{
          Swal.fire({
            title: 'Dodano nowy klucz !',
            confirmButtonText: `Ok`,
          }).then((result) => {
            Swal.close();
            this.dialogRef.close();
          })
        }, error =>  Swal.fire({
          title: 'Ups, Wystąpił problem spróbuj dodaćklucz jeszcze raz!',
          confirmButtonText: `Ok`,
        })
        )
      }
    }
}