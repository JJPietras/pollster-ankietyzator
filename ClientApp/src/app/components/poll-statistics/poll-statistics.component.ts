
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
      console.log(this.stats);
      this.stats.forEach(stat => {
        if (stat.type<2 || true ){
          this.graphs.push({
            data: [{
              values: stat.answerCounts.split("/"),
              labels: stat.options.split("/"),
              type: 'pie'
            }],
            layout: {
              title: "(" + stat.questionId + ") " + stat.title
            }
          })
        }
        else if (stat.type==2){
          this.graphs.push({
            type: 'table',
            header: {
              values: [["<b>EXPENSES</b>"], ["<b>Q1</b>"],
                   ["<b>Q2</b>"], ["<b>Q3</b>"], ["<b>Q4</b>"]],
              align: "center",
              line: {width: 1, color: 'black'},
              fill: {color: "grey"},
              font: {family: "Arial", size: 12, color: "white"}
            },
            cells: {
              values: stat.options.split("/"),
              align: "center",
              line: {color: "black", width: 1},
              font: {family: "Arial", size: 11, color: ["black"]}
            }
          })
        }
        else {
          this.graphs.push({
            data: [{
              values: stat.answerCounts.split("/"),
              labels: stat.options.split("/"),
              type: 'bar'
            }],
            layout: {
              title: "(" + stat.questionId + ") " + stat.title
            }
          })
        }
      });

      console.log(this.stats)
    }, error => console.error(error));
  }

}
