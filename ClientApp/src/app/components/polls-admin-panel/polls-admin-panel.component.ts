import { Component, OnInit, OnDestroy, Input, Inject } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { PollsService } from "../../services/polls-service";
import Swal from 'sweetalert2';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { PollsAdminPanelPopupComponent } from './polls-admin-panel-popup/polls-admin-panel-popup.component';

@Component({
  selector: 'app-polls-admin-panel',
  templateUrl: './polls-admin-panel.component.html',
  styleUrls: ['./polls-admin-panel.component.css']
})


export class PollsAdminPanelComponent implements OnInit {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
    private router: Router, public pollsService: PollsService, private route: ActivatedRoute,
    private dialog: MatDialog) {

  }

  searchTerm: string;
  pollsActiveA: PollStats[];
  pollsArchivedA: PollStats[];
  previewPoll: boolean;

  questionStats: QuestionStats[];
  pollStats: PollStats;
  pollDetailedAnswers: PollDetailedAnswers[];
  private pollId: number;

  ngOnInit() {
    this.getPollsData();
  }

  selectPoll(poll: PollStats) {

    this.pollsService.changePollStats(poll)
    this.pollId = poll.pollId;
    this.http.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.pollId).subscribe(result => {
      this.questionStats = result.data;
    }, error => console.error(error));

    this.loadDetails();

  }

  onShow(poll: PollStats) {
    this.selectPoll(poll);
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    dialogConfig.data = { n: this.pollId, pActive: this.pollsActiveA, pArchive: this.pollsArchivedA, pStats: this.pollStats };
    this.dialog.open(PollsAdminPanelPopupComponent, dialogConfig).afterClosed().subscribe(result => {
    });
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


  loadDetails() {
    this.http.get<Request>(this.baseUrl + 'answers/get-detailed-answers/' + this.pollId).subscribe(result => {
      this.pollDetailedAnswers = result.data;
    });
  }

  loadStatistics() {
    this.pollStats = this.pollsService.pollStatsSource.value;
  }

  selectPoll2(poll: PollStats) {
    this.pollsService.changePollStats(poll)
    this.router.navigate(['/poll-statistics/' + poll.pollId])
  }

}
