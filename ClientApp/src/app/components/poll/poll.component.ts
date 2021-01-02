
import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import Swal from 'sweetalert2';
import { PollsService } from "../../services/polls-service";

@Component({
  selector: 'app-poll',
  templateUrl: './poll.component.html',
  styleUrls: ['./poll.component.scss']
})

export class PollComponent{

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute, public pollsService: PollsService) {
    
  }
  poll: Poll;
  answersFilled: boolean;
  answers: Answer[];

  ngOnInit() {
    this.poll = this.pollsService.pollSource.value;
    this.poll.questions.forEach(q => {
      if (q.type==2)
        q.answer = new Array<number>(q.options.split('/').length)
    });
  }

  submitPoll(){
    this.reformatAndAccept();

    if (this.answersFilled){
      this.http
      .post<Answer[]>(this.baseUrl + "answers/add-answers", this.answers)
      .subscribe(
        (result) => {
          console.log(result);
          Swal.fire("Dziękujemy", "Dodano odpowiedzi", "info").then(
            () => {
              this.router.navigate(['/'])
            }
          );
        },
        (error) => {
          Swal.fire("Błąd", error.message, "error");
        }
      );
    }
    else{
      Swal.fire("Nie można przesłać odpowiedzi", "Wypełnij wszystkie pytania", "error");
    }
  }

  reformatAndAccept(){
    this.answers = [];
    this.answersFilled = true;
    this.poll.questions.forEach(
      q => {
        if (q.type==2){
            var finalAnswer = []
            q.answer.forEach((element, index) => {
              if (element)
                finalAnswer.push(index);
            });
              this.answers.push({questionId: q.questionId, content: finalAnswer.join("/")});
              this.checkAnswer(finalAnswer);
          }
          else{
            this.answers.push({questionId: q.questionId, content: q.answer.toString()});
            this.checkAnswer(q.answer);
          }
        }
    )
    console.log(this.answers);
  }

  checkAnswer(answer: any){
    if (!answer || answer == "")
      this.answersFilled = false
  }

}
