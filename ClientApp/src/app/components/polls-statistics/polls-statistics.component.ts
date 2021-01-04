
import {Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';


@Component({
  selector: 'app-polls-statistics',
  templateUrl: './polls-statistics.component.html',
  styleUrls: ['./polls-statistics.component.scss']
})

export class PollsStatisticsComponent implements OnInit {

  public graphPie: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router) {

  }

  stats: PollStats[];

  ngOnInit() {
    this.http.get<Request>(this.baseUrl + 'stats/get-polls-stats').subscribe(result => {
     this.stats = result.data;

     this.graphPie = {
        data: [{
          values: this.stats.map(stat => stat.completions),
          labels: this.stats.map(stat => "(" + stat.pollId.toString() + ") " + stat.title),
          type: 'pie'
        }],
        layout: {
          title: 'Udział wypełnionych ankiet'
        }
      };
    }, error => console.error(error));
  }

  selectPoll(pollId: number) {
    this.router.navigate(['/poll-statistics/' + pollId])
  }

}
