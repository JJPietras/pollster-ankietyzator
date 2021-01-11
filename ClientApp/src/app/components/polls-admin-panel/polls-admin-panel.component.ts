import { Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { map } from "rxjs/operators";
import { forkJoin } from 'rxjs';
import { PollsService } from "../../services/polls-service";
import Swal from 'sweetalert2';


@Component({
  selector: 'app-polls-admin-panel',
  templateUrl: './polls-admin-panel.component.html',
  styleUrls: ['./polls-admin-panel.component.css']
})





export class PollsAdminPanelComponent implements OnInit {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
   private router: Router, private pollsService: PollsService, private route: ActivatedRoute) { 

    this.pollId = Number(this.route.snapshot.paramMap.get('id'));
    //this.loadDetails();
  }
  

   searchTerm: string;
   pollsActiveA: PollStats[];
   pollsArchivedA: PollStats[];
   previewPoll: boolean;

   

   pollStats: PollStats;
   pollDetailedAnswers: PollDetailedAnswers[];
   private pollId: number;



  ngOnInit() {
    this.getPollsData();
    this.loadDetails();
  }

  pokaz(){
    console.log(this.pollDetailedAnswers);
  }

  selectPoll(poll: PollStats) {
    this.pollsService.changePollStats(poll)
    if(!this.previewPoll){
      this.previewPoll = true;
    }else{
      this.previewPoll = false;
    }
      
    /*Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć klucz ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          
  
      }
    );*/
    //this.router.navigate(['/poll-statistics/' + poll.pollId])
  }

  getPollsData() {
    let r1 = this.http.get<Request>(this.baseUrl + 'polls/get-un-archived');
    let r2 = this.http.get<Request>(this.baseUrl + 'polls/get-archived');
    let r3 = this.http.get<Request>(this.baseUrl + 'stats/get-polls-stats');

    forkJoin([r1, r2, r3]).subscribe(result => {
      const [active, archived, stats] = result;

      this.pollsActiveA = active.data.map(item => {
        const obj = stats.data.find(o => o.pollId === item.pollId);
        return { ...item, ...obj };
      });

      this.pollsArchivedA = archived.data.map(item => {
        const obj = stats.data.find(o => o.pollId === item.pollId);
        return { ...item, ...obj };
      });
    }, error => console.error(error));
  }


  loadDetails(){
    //if (this.pollStats.nonAnonymous){
      this.http.get<Request>(this.baseUrl + 'answers/get-detailed-answers/' + this.pollId).subscribe(result => {
        this.pollDetailedAnswers = result.data;
      });
   // }
   // else{
     // this.http.get<Request>(this.baseUrl + 'answers/get-anonymous-answers/' + this.pollId).subscribe(result => {
       // this.pollAnonymousAnswers = result.data;
     // });
   // }
  }


  loadStatistics(){
    this.pollStats = this.pollsService.pollStatsSource.value;
  
  }


  deletePoll(poll : Poll){


    let timerInterval;

    Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć ankietę ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          this.http.delete<Poll>(this.baseUrl + 'polls/remove-poll/' + poll.pollId).subscribe(result =>{
            console.log(result);
          }, error => console.log(error));
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

    archivePoll(poll : Poll){
      //close-poll/

      let timerInterval;

    Swal.fire({
      showDenyButton: true,
      title: `Zarchiwizować ankietę ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
    .then(
      (result) => {
        if (result.isConfirmed) {
          this.http.put<Poll>(this.baseUrl + 'polls/close-poll/' + poll.pollId, poll).subscribe(result =>{
            console.log(result);
          }, error => console.log(error));
          
            Swal.fire({
              title: 'Ankieta zarchiwizowana.',
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
  

}
