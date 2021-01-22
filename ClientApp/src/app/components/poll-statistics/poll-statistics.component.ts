
import {Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { PollsService } from "../../services/polls-service";
import { isNumber } from '@ng-bootstrap/ng-bootstrap/util/util';

@Component({
  selector: 'app-poll-statistics',
  templateUrl: './poll-statistics.component.html',
  styleUrls: ['./poll-statistics.component.scss']
})

export class PollStatisticsComponent implements OnInit {
  private pollId: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute, private pollsService: PollsService) {
    this.pollId = Number(this.route.snapshot.paramMap.get('id'));
  }

  questionStats: QuestionStats[];
  graphs: any;

  pollStats: PollStats;

  pollDetailedAnswers: PollDetailedAnswers[];

  pollAnonymousAnswers: Answer[];

  ngOnInit() {
    this.loadStatistics();
    this.loadDetails();
  }


  loadStatistics(){
    this.pollStats = this.pollsService.pollStatsSource.value;

    this.http.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.pollId).subscribe(result => {
      this.questionStats = result.data;
      this.formatData()
    }, error => console.error(error));
  }

  formatData(){
    this.graphs = [];
    let counts = [];
    console.log(this.questionStats)
    this.questionStats.forEach(stat => {
      if (stat.type<2 || stat.type==2 || stat.type==4  ){
        counts = stat.answerCounts.split("/")

        if (this.checkForAnswers(counts)){
          this.graphs.push({
            data: [{
              values: counts,
              labels: stat.options.split("/"),
              type: 'pie'
            }],
            layout: {
              title: "(" + stat.questionId + ") " + stat.title
            }
          })
        }
        else{
          this.graphs.push({title: "(" + stat.questionId + ") " + stat.title, counts: stat.answerCounts})
        }
      }
      else if (stat.type==3){
        this.graphs.push({title: "(" + stat.questionId + ") " + stat.title, counts: stat.answerCounts})
        // this.graphs.push({
        //   type: 'table',
        //   header: {
        //     values: [["<b>EXPENSES</b>"], ["<b>Q1</b>"],
        //          ["<b>Q2</b>"], ["<b>Q3</b>"], ["<b>Q4</b>"]],
        //     align: "center",
        //     line: {width: 1, color: 'black'},
        //     fill: {color: "grey"},
        //     font: {family: "Arial", size: 12, color: "white"}
        //   },
        //   cells: {
        //     values: stat.options.split("/"),
        //     align: "center",
        //     line: {color: "black", width: 1},
        //     font: {family: "Arial", size: 11, color: ["black"]}
        //   }
        // })
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
  }

  loadDetails(){
    if (this.pollStats.nonAnonymous){
      this.http.get<Request>(this.baseUrl + 'answers/get-detailed-answers/' + this.pollId).subscribe(result => {
        this.pollDetailedAnswers = result.data;
      });
    }
    else{
      this.http.get<Request>(this.baseUrl + 'answers/get-anonymous-answers/' + this.pollId).subscribe(result => {
        this.pollAnonymousAnswers = result.data;
      });
    }
  }

  getAnswers(questionId: number){
    return this.pollAnonymousAnswers.filter(a => a.questionId == questionId);
  }

  checkForAnswers(arr) { 
    for(let x of arr) {
        if(x > 0) return true
    }
    console.log(typeof arr)
    if ( typeof arr == 'number') return true
    return false
}
}
