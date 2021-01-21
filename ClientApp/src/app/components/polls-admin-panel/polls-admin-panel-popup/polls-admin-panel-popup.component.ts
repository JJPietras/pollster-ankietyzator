import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { PollsService } from 'src/app/services/polls-service';
import { SettingsService } from 'src/app/services/settings.service';
import { PollsAdminPanelComponent } from '../polls-admin-panel.component';

@Component({
  selector: 'app-polls-admin-panel-popup',
  templateUrl: './polls-admin-panel-popup.component.html',
  styleUrls: ['./polls-admin-panel-popup.component.css']
})
export class PollsAdminPanelPopupComponent implements OnInit {


  receivedPollId;
  receivedPollsActive;
  receivedPollsArchive;
  pollStats;

   questionStats: QuestionStats[];
   //pollStats: PollStats;
   pollDetailedAnswers: PollDetailedAnswers[];
   

  constructor(public dialogRef: MatDialogRef<PollsAdminPanelComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,  public httpclient: HttpClient, 
    @Inject('BASE_URL') public baseUrl: string, private router: Router,
    public pollsService: PollsService) { 
      this.receivedPollId = data.n; 
      this.receivedPollsActive = data.pActive;
      this.receivedPollsArchive = data.pArchive;

      this.loadDetails();
     this.pollStats = this.pollsService.pollStatsSource.value;
    }



  ngOnInit() {
    this.loadDetails();
    this.pollStats = this.pollsService.pollStatsSource.value;
    console.log(this.pollStats);
    
    //this.loadStatistics();
  }


  selectPoll(poll: PollStats) {
    this.pollsService.changePollStats(poll)
    this.receivedPollId = poll.pollId;
    this.httpclient.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.receivedPollId).subscribe(result => {
      this.questionStats = result.data;
    }, error => console.error(error));
  }

  loadDetails(){
    this.httpclient.get<Request>(this.baseUrl + 'answers/get-detailed-answers/' + this.receivedPollId).subscribe(result => {
      this.pollDetailedAnswers = result.data;
    });
/*
    this.httpclient.get<Request>(this.baseUrl + 'answers/get-anonymous-answers/' + this.receivedPollId).subscribe(result => {
      this.pollDetailedAnswers += result.data;
    });*/
    
  }

  onClose(){
    this.dialogRef.close();
    console.log("odpowiedzi: " + this.pollDetailedAnswers);
  }

  /*
  loadStatistics(){
    this.pollStats = this.pollsService.pollStatsSource.value;
  }*/


  

}
