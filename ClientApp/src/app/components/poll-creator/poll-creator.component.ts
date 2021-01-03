import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import { FormArray, FormBuilder, FormGroup, MaxLengthValidator, Validators } from '@angular/forms';
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
   private router: Router, private route: ActivatedRoute, private fb: FormBuilder, 
   private authenticationService: AuthenticationService, private pollService: PollsService) {

  }

  ngOnInit() {
   
  }
  
  
  // private questions: Question[];
   private charsMaxNumber = 5555;
   private dataSource: Array<Question> = [];

   private pollTags: string = "";
   private pollEmailsUsers: string ="";
   private pollDescription = "opis";
   private pollTitle = "Tytuł";
   //########
   
  /*private newPoll: Poll ={
    pollId: undefined,
    authorId: undefined,
    authorEmail: undefined,
    authorName: undefined,
    title: undefined,
    tags: undefined,
    emails: undefined,
    nonAnonymous: undefined,
    archived: undefined,
    questions: undefined
   };*/

   private newPoll: Poll2 ={
    pollId: undefined,
    authorId: undefined,
    description: undefined,
    title: undefined,
    tags: undefined,
    emails: undefined,
    nonAnonymous: undefined,
    archived: undefined,
    questions: undefined
   };

   
   setType(val: any ,inx: any){
    console.log(val);
    this.dataSource[inx].type = val;
  }

  submit(){
    let it = 0;
    this.dataSource.forEach(q => {
      if(q.allowEmpty == undefined)
        q.allowEmpty = Boolean(0);

      q.position = it;
      q.maxLength = this.charsMaxNumber;
      
      console.log(q.title + " " + q.allowEmpty + " " + q.position + " " +  q.options + " " + q.maxLength + " " + q.type);
      it++;
    })

    
    this.newPoll.authorId = this.authenticationService.user.value.accountId;
    console.log(this.pollTitle);
    this.newPoll.title = this.pollTitle;
    this.newPoll.description = this.pollDescription;
    this.newPoll.tags = this.pollTags;
    this.newPoll.emails = this.pollEmailsUsers;
    this.newPoll.nonAnonymous = true;
    this.newPoll.archived = false;
    this.newPoll.questions = this.dataSource;

  
    console.log(this.authenticationService.user.value);
    this.postPoll(this.newPoll);


  }

  
  postPoll(poll: Poll2){
    if(poll){
    this.http
      .post<Poll2>(this.baseUrl + "polls/create-poll", poll)
      .subscribe(
        (result) => {
          console.log(result);
          Swal.fire("Gratulacje", "utworzono ankietę", "info").then(
            () => {
              this.router.navigate(['/'])
            }
          );
        },
        (error) => {
          Swal.fire("Błąd", error.message, "error");
          console.log(error.message);
        }
      );
    }
    else{
      Swal.fire("Nie można przesłać odpowiedzi", "?", "error");
      
    }
  }


  changedCharMaxNumber(){
    if(this.charsMaxNumber <= 0)
      this.charsMaxNumber = 5555;

      this.dataSource.forEach(q =>{
        q.maxLength = this.charsMaxNumber;
      })

      console.log(this.charsMaxNumber);
  }


  addQuery(){
      let div = new Query();
      this.dataSource.push(div);    
  }

  removeDiv(val : any){
    this.dataSource.splice(val, 1);
  }


  //#########################################
  //#########################################
  //#########################################
  // dynamiczne inputy narazie nie uzywane  
  answersForm = this.fb.group({
    answers: this.fb.array([
      this.fb.control('')
    ])
  })
  
  
  get answers(){
    return this.answersForm.get('answers') as FormArray;
  }

  
  addNewAnswer(){
    this.answers.push(this.fb.control(''));
    //console.log("odpowiedź: " + this.answersForm.get(['answers','0']).value);
  }

  removeAnswer(){
    if(this.answers.length > 1)
      this.answers.removeAt(this.answers.length-1);
  }
//#######################################################
//#######################################################
//#######################################################

}

export class Query implements Question{
  questionId: number;
  position: number;
  title: string;
  options: string;
  allowEmpty: boolean;
  maxLength: number;
  type: number;
  answer: any;
  fB: FormGroup;
}


