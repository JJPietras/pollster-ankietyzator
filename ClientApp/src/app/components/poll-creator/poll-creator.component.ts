import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { PollsService } from 'src/app/services/polls-service';
import Swal from 'sweetalert2';



@Component({
  selector: 'app-poll-creator',
  templateUrl: './poll-creator.component.html',
  styleUrls: ['./poll-creator.component.css']
})

export class PollCreatorComponent implements OnInit {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
   private router: Router, private authenticationService: AuthenticationService) {
  }

  questionsCreator: NewQuestionCreator[]=[];
  newPoll: NewPoll;

  ngOnInit() {
    this.newPoll={
      title: "",
      authorId: undefined,
      description: "",
      tags: "",
      emails:"",
      nonAnonymous: false,
      archived: false,
      questions: [],
    }
  }
  
  setType(type: any, index: any){
    var options: any = [];
    if (type == 3 || type == 4)
      options= Array(3);
    else if (type == 2)
      options = "";
   this.questionsCreator[index].options = options;
   this.questionsCreator[index].type = type;
  }

  submit(){
    this.newPoll.questions = []
    this.newPoll.authorId = this.authenticationService.user.value.accountId;
    this.questionsCreator.forEach(question => {
      this.newPoll.questions.push(this.convertQuestion(question))
    });
    console.log(this.newPoll);
    this.postPoll()
  }

  removeQuestion(index: number){
    this.questionsCreator.splice(index, 1);
  }

  addQuestion(){
    this.questionsCreator.push(this.emptyQuestion(this.questionsCreator.length+1))
  }
  
  postPoll(){
    if(this.newPoll){
      this.http
        .post<NewPoll>(this.baseUrl + "polls/create-poll", this.newPoll)
        .subscribe(
          (result) => {
            console.log(result);
            Swal.fire("Gratulacje", "utworzono ankietę", "info").then(
              () => { this.router.navigate(['/'])}
            );
          },
          (error) => {
            Swal.fire("Błąd", error.message, "error");
            console.log(error.message);
          }
        );
    }
    else{
      Swal.fire("Nie można przesłać odpowiedzi.", "", "error");
    }
  }
  
  convertQuestion(question: NewQuestionCreator): NewQuestion{
    var options: string;

    if (question.type == 2)
      options = question.options;
    else 
      options = question.options.join("/");

    return {
      position: question.position,
      title: question.title,
      options:  options,
      allowEmpty: question.allowEmpty,
      maxLength: question.maxLength,
      type: question.type
    }
  }

   emptyQuestion(index: number): NewQuestionCreator {
    return {
      position: index,
      title: "",
      options: "",
      allowEmpty: false,
      maxLength: 0,
      type: undefined,
      helpText: ""
    }
  }

  addOption(question: number, text: string){
    if (text.trim() == ""){
      Swal.fire("Nie można dodać pustego pytania.", "", "error");
    }
    else{
      this.questionsCreator[question].options.push(text);
      //console.log(this.questionsCreator);
      this.questionsCreator[question].helpText = ""
    }
  }

  deleteOption(question: number, option: number){
    this.questionsCreator[question].options.splice(option, 1);
  }

  validate(){
    //console.log(this.newPoll);
    var form = document.getElementsByClassName('needs-validation')[0] as HTMLFormElement;
    var optionsAdded = true;
    var typesAdded = true;
    this.questionsCreator.forEach(q => {
      if (q.type!=null){
        if (q.type < 2)
          if (q.options.length < 2 || !q.title || q.title == '')
            optionsAdded= false;
      }
      else
        typesAdded = false
      });

    if (this.questionsCreator.length == 0){
      Swal.fire("Dodanie pytań jest wymagane.", "", "error");
    }
    else if (form.checkValidity() === false) {
      event.preventDefault();
      event.stopPropagation();
      Swal.fire("Uzupełnij wszystkie wymagane pola.", "", "error");
    }
    else if (!optionsAdded){
      Swal.fire("Uzupełnij pytania prawidłowo", "", "error");
    }
    else if (!typesAdded){
      Swal.fire("Wybierz typy dla wszystkich pytań.", "", "error");
    }
    else{
      this.submit()
    }
    form.classList.add('was-validated');
  }

}
