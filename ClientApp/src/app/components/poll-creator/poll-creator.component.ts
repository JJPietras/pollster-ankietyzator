import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';  
import { AuthenticationService } from 'src/app/services/authorisation.service';
import { PollsService } from 'src/app/services/polls-service'; 
import { QuestionType } from 'src/app/models/question-type.model';
import Swal from 'sweetalert2';



@Component({
  selector: 'app-poll-creator',
  templateUrl: './poll-creator.component.html',
  styleUrls: ['./poll-creator.component.css']
})

export class PollCreatorComponent implements OnInit {
  questionsCreator: NewQuestionCreator[]=[];
  newPoll: NewPoll;
  submited = false;

  type = "new";
  
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
   private router: Router, private authenticationService: AuthenticationService, private pollsService: PollsService, private route: ActivatedRoute) {
    this.type = this.route.snapshot.paramMap.get('type');

    if (this.type=="edit"){
      this.newPoll=pollsService.newPollSource.value;
      this.newPoll.questions.forEach(q => {
        let opt = (q.type==3)? q.options : q.options.split("/")
        let temp: NewQuestionCreator = {
          position: q.position,
          title: q.title,
          options: opt,
          allowEmpty: q.allowEmpty,
          maxLength: q.maxLength,
          type: q.type,
          helpText: "",
      };
        this.questionsCreator.push(temp)
      })
    }
    else {
      this.newPoll={
        title: "",
        authorId: undefined,
        description: "",
        tags: "",
        emails: "",
        nonAnonymous: false,
        archived: false,
        questions: [],
        newEmail: "",
        newEmails: [],
        newTag: "",
        newTags: []
      }
    }
  }

  ngOnInit() {
    
  }
  
  setType(type: any, index: any){
    let options: any = [];
    if (type == 2 || type == 4)
      options= Array(3);
    else if (type == 3)
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
    this.newPoll.emails = this.newPoll.newEmails.join("/");
    this.newPoll.tags = this.newPoll.newTags.join("/");
    console.log(this.newPoll);

    if (this.type=='new')
      this.postPoll()
    else if (this.type=='edit')
      this.putPoll()
  }

  removeQuestion(index: number){
    this.questionsCreator.splice(index, 1);
  }

  addQuestion(){
    this.questionsCreator.push(this.emptyQuestion(this.questionsCreator.length+1))
  }
  
  postPoll(){
    if(this.newPoll && !this.submited){
      this.submited = true;
      this.pollsService.showLoading("Tworzenie ankiety.")
      this.http
        .post<NewPoll>(this.baseUrl + "polls/create-poll", this.newPoll)
        .subscribe(
          (result) => {
            //console.log(result);
            Swal.close();
            Swal.fire("Gratulacje", "utworzono ankietę", "info").then(
              () => { this.router.navigate(['/'])}
            );
          },
          (error) => {
            Swal.close();
            Swal.fire("Błąd", error.message, "error");
            console.log(error.message);
            this.submited = false;
          }
        );
    }
    else{
      Swal.fire("Nie można przesłać odpowiedzi.", "", "error");
    }
  }
  
  putPoll(){
    if(this.newPoll && !this.submited){
      this.submited = true;
      this.pollsService.showLoading("Aktualizacja ankiety.")
      
      this.http
        .put<NewPoll>(this.baseUrl + "polls/update-poll", this.newPoll)
        .subscribe(
          (result) => {
            
            Swal.close();
            Swal.fire("Gratulacje", "zaktualizowano ankietę", "info").then(
              () => { this.router.navigate(['/'])}
            );
          },
          (error) => {
            Swal.close();
            Swal.fire("Błąd", error.message, "error");
            console.log(error.message);
            this.submited = false;
          }
        );
    }
    else{
      Swal.fire("Nie można przesłać odpowiedzi.", "", "error");
    }
  }

  convertQuestion(question: NewQuestionCreator): NewQuestion{
    let options: string;

    if (question.type == 3)
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

  deleteMail(option: number){
    this.newPoll.newEmails.splice(option, 1);
  }

  addMail(){
    if (this.newPoll.newEmail.trim() == ""){
      Swal.fire("Podaj poprawny E-mail.", "", "error");
    }
    else{
      this.newPoll.newEmails.push(this.newPoll.newEmail);
      //console.log(this.questionsCreator);
      this.newPoll.newEmail = ""
    }
  }

  deleteTag(option: number){
    this.newPoll.newTags.splice(option, 1);
  }

  addTag(){
    if (this.newPoll.newTag.trim() == ""){
      Swal.fire("Podaj poprawny tag.", "", "error");
    }
    else{
      this.newPoll.newTags.push(this.newPoll.newTag);
      //console.log(this.questionsCreator);
      this.newPoll.newTag = ""
    }
  }

  validate(){
    //console.log(this.newPoll);
    let form = document.getElementsByClassName('needs-validation')[0] as HTMLFormElement;
    let optionsAdded = true;
    let typesAdded = true;
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
      Swal.fire("Uzupełnij pytania prawidłowo.", "", "error");
    }
    else if (!typesAdded){
      Swal.fire("Wybierz typy dla wszystkich pytań.", "", "error");
    }
    else if (this.newPoll.newEmails.length<1 && this.newPoll.newTags.length<1){
      Swal.fire("Uzupełnij tagi lub maile.", "", "error");
    }
    else{
      this.submit()
    }
    form.classList.add('was-validated');
  }

}
