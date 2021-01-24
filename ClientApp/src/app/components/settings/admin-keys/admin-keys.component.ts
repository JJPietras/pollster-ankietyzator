import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { AuthenticationService } from "../../../services/authorisation.service";
import Swal from 'sweetalert2';
import { SettingsService } from 'src/app/services/settings.service';
import { UpdateAccountDto } from 'src/app/models/updateDTO.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { from, Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import {MatDialog, MatDialogConfig} from '@angular/material';
import { AdminEditKeyComponent } from './admin-edit-key/admin-edit-key.component';


@Component({
  selector: 'app-admin-keys',
  templateUrl: './admin-keys.component.html',
  styleUrls: ['./admin-keys.component.css']
})
export class AdminKeysComponent {

  emailFilter: string = "";
  keyUserTypeFilter: number = -1;

  keys: Key[];

  constructor(public authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router
    ,public settingsService: SettingsService, private dialog: MatDialog) {

    this.getKeys();

    //  this.http.get<Request>(this.baseUrl + 'accounts/get-accounts').subscribe(result => {
    //    this.UsersAccounts = result.data;
    //  })
     
     console.log(this.authenticationService.user.value);
  }

  getKeys(){
    this.keys = undefined;
    this.http.get<Request>(this.baseUrl + 'keys/get-keys').subscribe(result =>{
      this.keys = result.data;
      this.settingsService.changeKeys(this.keys)
      console.log(this.keys);
   }, (error) => { console.log(error.message); })
  }

  public editKey(key: Key){
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = false;
      dialogConfig.autoFocus = true;
      dialogConfig.width = "60%";
      dialogConfig.data = key;
      this.dialog.open(AdminEditKeyComponent, dialogConfig).afterClosed().subscribe(result =>{
        this.getKeys();
      });
  }

  public removeKey(val:any){
    Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć klucz ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          this.settingsService.showLoading("Usuwanie klucza.");
          
          this.http.delete(this.baseUrl + 'keys/remove-key/' + val).subscribe(result =>{
            console.log(result);
            Swal.close();
            this.getKeys();
          },  (error) => {
            Swal.close();
            Swal.fire("Błąd", error.message, "error");
            console.log(error.message);
          });
        } 
      }
    );
  }

}