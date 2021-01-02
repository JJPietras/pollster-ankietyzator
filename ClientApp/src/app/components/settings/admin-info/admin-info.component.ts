import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Router } from '@angular/router'
import { AuthenticationService } from "../../../services/authorisation.service";
import { MatTableDataSource } from '@angular/material';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-admin-info',
  templateUrl: './admin-info.component.html',
  styleUrls: ['./admin-info.component.css']
})
export class AdminInfoComponent implements OnInit {

  //private userSource:BehaviorSubject<User>;
  //currentUsers:Observable<User>;
  rows: Number = 10; 
  namesAccounts: string[] = [];
  UsersAccounts: User[];
  filteredUsersAccounts: User[];
  currentModifyUser: Number = -1;
  oneUser: User ;
  objectProperty: Object[];
  objectCurrentProperty: Object;
  contentProperty: string = "";
  popup: boolean;

  dataSource;
  objectProperty2: any[];



  constructor(public authenticationService: AuthenticationService, public httpclient: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router) {

     this.UsersAccounts = this.authenticationService.users.value;
     console.log(this.authenticationService);
     this.dataSource = new MatTableDataSource(this.UsersAccounts);
     this.filteredUsersAccounts = this.UsersAccounts;
     
  }

  ngOnInit(){
    
  }

  /*get users(){
    return this.usersSource;
  }*/

  public onItemSelected(val: any){

    this.currentModifyUser  = Number(val);
    this.oneUser = val;
    this.objectProperty = [];

    this.objectProperty = Object.keys(this.oneUser);
    this.objectProperty2 = Object.values(this.oneUser);
    
    this.objectProperty.forEach(value =>{
      if(value.toString() === "accountId" || value.toString() ==="eMail"){
        this.objectProperty2.splice(this.objectProperty.indexOf(value),1);
        this.objectProperty.splice(this.objectProperty.indexOf(value),1);
        
      }
      this.contentProperty = "";
     
    })

  }


  public onItemSelectedProperty(val: any){

    this.objectCurrentProperty = this.objectProperty2[val];
    this.contentProperty = this.objectProperty2[val];
    
    console.log("tu prpoperty: " + this.objectProperty2[val]);
   
  }



  public filterEmail(val :any){
    
    this.filteredUsersAccounts = [];
    this.UsersAccounts.filter(user =>{
  
      if(user.eMail.includes(val) && (val.length != 0)){
        
        console.log(user);
        this.filteredUsersAccounts.push(user);
        
      }
    })

    if(val.length === 0 || val.trim() == ""){
      console.log(val.trim() != "")
      this.filteredUsersAccounts = this.UsersAccounts;
    }
 }

 public filterTypeUser(val: any){
  this.filteredUsersAccounts = [];
  this.UsersAccounts.filter(user =>{
      console.log(val);
      if(val == 0){
        console.log(val);
        this.filteredUsersAccounts.push(user);
      }
      else if(val == 1){
        this.filteredUsersAccounts.push(user);
      }
      else if(val == 2){
        this.filteredUsersAccounts.push(user);
      }
  })

  /*if(val.length === 0 || val.trim() == ""){
    console.log(val.trim() != "")
    this.filteredUsersAccounts = this.UsersAccounts;
  }*/

 }

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



}

