
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

  stats: QuestionStats[];
  graphs: any;


  ngOnInit() {
    this.http.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.pollId).subscribe(result => {
      this.stats = result.data;
      this.graphs = [];
      this.stats.forEach(stat => {
        
        this.graphs.push({
          data: [{
            values: stat.answerCounts.split("/"),
            labels: stat.answerCounts.split("/"),
            type: 'pie' //(a < b) ? 'pie' : 'bar'
          }],
          layout: {
            title: stat.questionId
          }
        })

      });

      console.log(this.stats)
    }, error => console.error(error));
  }

}
