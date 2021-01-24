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
import { AdminEditUserComponent } from './admin-edit-user/admin-edit-user.component';


@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent {

  emailFilter: string = "";
  keyUserTypeFilter: number = -1;

  constructor(public authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public settingsService: SettingsService, private dialog: MatDialog) {
    
  }

  public editUser(user: User){
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = false;
      dialogConfig.autoFocus = true;
      dialogConfig.width = "60%";
      dialogConfig.data = user;
      this.dialog.open(AdminEditUserComponent, dialogConfig).afterClosed().subscribe(result =>{
        this.settingsService.reloadKeys();
        this.settingsService.reloadUsers();
      });
  }

}