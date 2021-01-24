
import { Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { map } from "rxjs/operators";
import { forkJoin } from 'rxjs';
import { PollsService } from "../../services/polls-service";

@Component({
  selector: 'app-polls-statistics',
  templateUrl: './polls-statistics.component.html',
  styleUrls: ['./polls-statistics.component.scss']
})

export class PollsStatisticsComponent implements OnInit {

  public graphActive: any;
  public graphArchived: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, public pollsService: PollsService) {

  }

  pollsActive: PollStats[];
  pollsArchived: PollStats[];

  ngOnInit() {
    this.getPollsData()

  }

  selectPoll(poll: PollStats) {
    this.pollsService.changePollStats(poll)
    this.router.navigate(['/poll-statistics/' + poll.pollId])
  }

  getPollsData() {
    let r1 = this.http.get<Request>(this.baseUrl + 'polls/get-pollster-un-archived');
    let r2 = this.http.get<Request>(this.baseUrl + 'polls/get-pollster-archived');
    let r3 = this.http.get<Request>(this.baseUrl + 'stats/get-polls-stats');

    forkJoin([r1, r2, r3]).subscribe(result => {
      const [active, archived, stats] = result;

      this.pollsActive = active.data.map(item => {
        const obj = stats.data.find(o => o.pollId === item.pollId);
        return { ...item, ...obj };
      });

      this.pollsArchived = archived.data.map(item => {
        const obj = stats.data.find(o => o.pollId === item.pollId);
        return { ...item, ...obj };
      });

      this.graphActive = {
        data: [{
          values: this.pollsActive.map(stat => stat.completions),
          labels: this.pollsActive.map(stat => "(" + stat.pollId.toString() + ") " + stat.title),
          type: 'pie'
        }],
        layout: {
          title: 'Udział wypełnionych ankiet'
        }
      };

      this.graphArchived = {
        data: [{
          values: this.pollsArchived.map(stat => stat.completions),
          labels: this.pollsArchived.map(stat => "(" + stat.pollId.toString() + ") " + stat.title),
          type: 'pie'
        }],
        layout: {
          title: 'Udział wypełnionych ankiet'
        }
      };
    }, error => console.error(error));

  }

  archivePoll(pollId: number){

  }

}
