
import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import Swal from 'sweetalert2';
import { PollsService } from "../../services/polls-service";

@Component({
  selector: 'app-poll',
  templateUrl: './poll.component.html',
  styleUrls: ['./poll.component.scss'],
  
})

export class PollComponent{

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute, public pollsService: PollsService) {
    if (!this.pollsService.pollSource)
      this.router.navigate(['/select-poll'])
  }
  poll: Poll;
  answersFilled: boolean;
  answers: Answer[];

  ngOnInit() {
    this.poll = this.pollsService.pollSource.value;
    this.poll.questions.forEach(q => {
      if (q.type==1)
        q.answer = new Array<number>(q.options.split('/').length)
    });
  }

  submitPoll(){

    this.reformatAndAccept();

    if (this.answersFilled && this.validate()){
      this.http
      .post<Answer[]>(this.baseUrl + "answers/add-answers", this.answers)
      .subscribe(
        (result) => {
          console.log(result);
          Swal.fire("Dziękujemy.", "Dodano odpowiedzi.", "info").then(
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
      Swal.fire("Nie można przesłać odpowiedzi", "Wypełnij wszystkie pytania poprawnie", "error");
    }
  }

  reformatAndAccept(){
    this.answers = [];
    this.answersFilled = true;
    this.poll.questions.forEach(
      q => {
        if (q.type==1){
            let finalAnswer = []
            q.answer.forEach((element, index) => {
              if (element)
                finalAnswer.push(index);
            });
            this.checkAnswer(finalAnswer, q.allowEmpty);
            if (finalAnswer.length>0)
              this.answers.push({questionId: q.questionId, content: finalAnswer.join("/")});
          }
          else{
            this.checkAnswer(q.answer, q.allowEmpty);
            if (q.answer)
              this.answers.push({questionId: q.questionId, content: q.answer.toString()});
          }
        }
    )
    //console.log(this.answers);
  }

  checkAnswer(answer: any, allowEmpty: boolean){
    if (!allowEmpty){
      if (answer==null || answer == ""){
        this.answersFilled = false;
        //console.log(allowEmpty)
      }
    }
  }

  
  getStep(options: any): number{
    return (options.split('/')[2] < (options.split('/')[1] - options.split('/')[0]))? options.split('/')[2] : 1;
  }

  validate(): boolean{
    let form = document.getElementsByClassName('needs-validation')[0] as HTMLFormElement;
    let valid = true;
    if (form.checkValidity() === false) {
      event.preventDefault();
      event.stopPropagation();
      valid = false;
    }
    form.classList.add('was-validated');
    return valid;
  }

  checkboxValidate(answers: any[]): boolean{
    let res = false;
    answers.forEach(a => {
      if (a==true)
        res = true;
      })
    return res;
  }

  cancel(){
    this.router.navigate(['/select-poll'])
  }
}
