
import {Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-poll-statistics',
  templateUrl: './poll-statistics.component.html',
  styleUrls: ['./poll-statistics.component.scss']
})

export class PollStatisticsComponent implements OnInit {
  private pollId: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute) {
    this.pollId = Number(this.route.snapshot.paramMap.get('id'));
  }

  stats: PollStats[];

  ngOnInit() {
    this.http.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.pollId).subscribe(result => {
      this.stats = result.data;
      console.log(this.stats)
    }, error => console.error(error));
  }

  public graph = {
    data: [
        { x: [1, 2, 3], y: [2, 6, 3], type: 'scatter', mode: 'lines+points', marker: {color: 'red'} },
        { x: [1, 2, 3], y: [2, 5, 3], type: 'bar' },
    ],
    layout: { title: 'Bar chart'}
};

public graphPie = {
  data: [{
    values: [19, 26, 55],
    labels: ['Residential', 'Non-Residential', 'Utility'],
    type: 'pie'
  }],
  layout: {
    title: 'Pie chart'
  }
};

}
