
import { Component, Inject, Pipe, NgModule, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';

import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from '../../app.component';
import {Router} from '@angular/router'; 
import { PollsService } from 'src/app/services/polls-service'; 
import Swal from 'sweetalert2';

@Component({
  selector: 'app-clone-polls',
  templateUrl: './clone-polls.component.html'

})

@NgModule({
  imports: [
    BrowserModule,
    FormsModule
  ],
  declarations: [AppComponent]
})


export class ClonePollsComponent {

  constructor(public http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private pollsService: PollsService, private router: Router) {

  }

  clonePoll() {
    if (!this.pollsterId || !this.pollId || this.pollsterId <1 || this.pollId <1){
      Swal.fire("Błąd", "uzupełnij wszystkie pola poprawnie", "error")
    }
    else{
      this.pollsService.showLoading("Klonowanie ankiety.")
      this.http.get<Request>(this.baseUrl + 'polls/clone-poll/' + this.pollsterId + '/' + this.pollId).subscribe(result => {
        Swal.close()
        console.log(result)
        Swal.fire("Gratulacje", "ankieta została sklonowana", "info").then(
          () => { this.router.navigate(['/'])})
      }, error => { Swal.fire("Błąd", error.message, "error")});
    }
  }

  pollsterId: number;
  pollId: number;

}

