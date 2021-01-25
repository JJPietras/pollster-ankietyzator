import { CurrencyPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { SettingsService } from 'src/app/services/settings.service';
import Swal from 'sweetalert2';
import { AdminUsersComponent } from '../admin-users.component';

@Component({
  selector: 'app-admin-edit-user',
  templateUrl: './admin-edit-user.component.html'
})

export class AdminEditUserComponent  {

  user: User;

  tagsArray: string[] = new Array<string>();

  newTag: string;
  newKey: string = null;
  updateDTO: UpdateAccountDto;

  constructor(public dialogRef: MatDialogRef<AdminUsersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: User,  public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, public settingsService: SettingsService){
      if (data){
        this.user = data;
        this.tagsArray = this.user.tags.split("/");
      }
    } 
  
  saveChanges(){
    this.updateDTO = {
      Tags: this.tagsArray.join("/"),
      EMail: this.user.eMail,
      Key: this.newKey
    }
    console.log(this.updateDTO);
    this.settingsService.showLoading("Zapisywanie zmian.");
    this.httpclient.put<UpdateAccountDto>(this.baseUrl + 'accounts/update-other-account', this.updateDTO).subscribe(result =>{
      Swal.close();
      this.dialogRef.close();
    }, (error) => {
      Swal.close();
      Swal.fire("Błąd", error.message, "error");
      console.log(error.message);
    })
  }

  onClose() {
    this.dialogRef.close();
  }

  deleteTag(option: number) {
    this.tagsArray.splice(option, 1);
  }

  addTag() {
    if (this.newTag.trim() == "") {
      Swal.fire("Podaj poprawny tag.", "", "error");
    }
    else {
      this.tagsArray.push(this.newTag.split("/").join("-"));
      this.newTag = ""
    }
  }

  addKey() {
    Swal.fire({
      title: "Klucz uprawnień",
      text: "Podaj klucz:",
      input: 'text',
      showCancelButton: true
    }).then((result) => {
      if (result.value && result.value != "") {
        this.newKey = result.value;
      }
      else {
        this.newKey = null;
      }
    });
  }
}