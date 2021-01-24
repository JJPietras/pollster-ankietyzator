
import { Component, Inject, NgModule, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router'
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from '../../app.component';
import { AuthenticationService } from "../../../services/authorisation.service";
import { UpdateAccountDto } from '../../../models/updateDTO.model';
import { SettingsService } from 'src/app/services/settings.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html'
})

@NgModule({
  imports: [
    BrowserModule,
    FormsModule
  ],
  declarations: [AppComponent]
})

export class UserInfoComponent {

  tagsArray: string[] = new Array<string>();

  newTag: string;
  user: User;

  newKey: string = null;

  updateDTO: UpdateAccountDto;

  constructor(public authenticationService: AuthenticationService, public http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string, private router: Router, public settingService: SettingsService) {

    if (this.authenticationService.user) {
      this.user = this.authenticationService.user.value;
      this.tagsArray = this.user.tags.split('/');
    }
    else {
      this.refreshUser();
    }
  }

  refreshUser(){
    this.authenticationService.getUser();
    this.http.get<Request>(this.baseUrl + 'accounts/get-account').subscribe(result => {
      this.user = result.data;
      this.tagsArray = this.user.tags.split('/');
    }, error => {
      console.error("Failed to fetch the user session. Please, log in again.")
    });
  }

  deleteTag(option: number) {
    this.tagsArray.splice(option, 1);
  }

  addTag() {
    if (this.newTag.trim() == "") {
      Swal.fire("Podaj poprawny tag.", "", "error");
    }
    else {
      this.tagsArray.push(this.newTag);
      this.newTag = ""
    }
  }

  saveChanges() {
    this.updateDTO = {
      Tags: this.tagsArray.join("/"),
      EMail: this.user.eMail,
      Key: this.newKey
    }
    //console.log(this.updateDTO)
    if (this.updateDTO) {
      this.settingService.showLoading("Aktualizacja zmian")

      this.http.put<UpdateAccountDto>(this.baseUrl + "accounts/update-my-account", this.updateDTO).subscribe(result => {
        Swal.close()
        console.log(result);
        this.refreshUser();
      }, (error) => {
        Swal.close();
        Swal.fire("Błąd", error.message, "error");
        console.log(error.message);
      });
    }
  }

  addKey() {
    Swal.fire({
      title: "Klucz uprawnień",
      text: "Podaj klucz:",
      input: 'text',
      showCancelButton: true
    }).then((result) => {
      if (result.value || result.value == "") {
        this.newKey = result.value;
      }
      else {
        this.newKey = null;
      }
    });
  }
}
