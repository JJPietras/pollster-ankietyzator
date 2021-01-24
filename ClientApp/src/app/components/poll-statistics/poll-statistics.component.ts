
import { Component, OnInit, OnDestroy, Input, Inject, ViewChild, ElementRef } from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { PollsService } from "../../services/polls-service";
import { isNumber } from '@ng-bootstrap/ng-bootstrap/util/util';
import { AuthenticationService } from "../../services/authorisation.service";

@Component({
  selector: 'app-poll-statistics',
  templateUrl: './poll-statistics.component.html',
  styleUrls: ['./poll-statistics.component.scss']
})

export class PollStatisticsComponent implements OnInit {
  private pollId: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute, public pollsService: PollsService, public authenticationService: AuthenticationService) {
    this.pollId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.pollsService.pollStatsSource)
      this.router.navigate(['/poll-statistics'])
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


  loadStatistics() {
    this.pollStats = this.pollsService.pollStatsSource.value;

    this.http.get<Request>(this.baseUrl + 'stats/get-questions-stats/' + this.pollId).subscribe(result => {
      this.questionStats = result.data;
      this.formatData()
    }, error => console.error(error));
  }

  formatData() {
    this.graphs = [];
    let counts = [];
    //console.log(this.questionStats)
    this.questionStats.forEach(stat => {
      if (stat.type < 2) {
        counts = stat.answerCounts.split("/")

        if (this.checkForAnswers(counts)) {
          this.graphs.push({
            data: [{
              values: counts,
              labels: stat.options.split("/"),
              type: 'pie'
            }],
            layout: {
              title: "(" + stat.position + ") " + stat.title
            }
          })
        }
        else {
          this.graphs.push({ title: "(" + stat.position + ") " + stat.title, counts: stat.answerCounts })
        }
      }
      else if (stat.type == 3) {
        this.graphs.push({ title: "(" + stat.position + ") " + stat.title, counts: stat.answerCounts })
      }
      else {
        let values = [];
        let counts = [];

        stat.answerCounts.split("/").forEach(
          cnt => {
            values.push(Number(cnt.split(";")[0]));
            counts.push(Number(cnt.split(";")[1]));
          })

        this.graphs.push({
          data: [{
            x: values,
            y: counts,
            type: 'bar',
            marker: {
              color: 'rgb(49,130,189)',
              opacity: 0.7,
            }
          }],

          layout: {
            xaxis: { range: [stat.options.split("/")[0], stat.options.split("/")[1]] },
            title: "(" + stat.position + ") " + stat.title
          }
        })
      }
    });
  }

  loadDetails() {
    if (this.pollStats.nonAnonymous) {
      this.http.get<Request>(this.baseUrl + 'answers/get-detailed-answers/' + this.pollId).subscribe(result => {
        this.pollDetailedAnswers = result.data;
      });
    }
    else {
      this.http.get<Request>(this.baseUrl + 'answers/get-anonymous-answers/' + this.pollId).subscribe(result => {
        this.pollAnonymousAnswers = result.data;
      });
    }
  }

  getAnswers(questionId: number) {
    return this.pollAnonymousAnswers.filter(a => a.questionId == questionId);
  }

  checkForAnswers(arr) {
    for (let x of arr) {
      if (x > 0) return true
    }
    console.log(typeof arr)
    if (typeof arr == 'number') return true
    return false
  }

  editPoll() {

  }

}
