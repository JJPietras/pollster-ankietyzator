
import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import Swal from 'sweetalert2';
import { PollsService } from "../../services/polls-service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-poll-answers',
  templateUrl: './poll-answers.component.html',
  styleUrls: ['./poll-answers.component.scss'],
  
})

export class PollAnswersComponent{

  constructor(public pollsService: PollsService, private modalService: NgbModal) {
    
  }
  
  poll: PollStats;
  answers: PollDetailedAnswers;
  ngOnInit() {

  }

  @Input()
  set stats(stats: PollStats) {
    this.poll = stats;
  }

  @Input()
  set answer(answer: PollDetailedAnswers) {
    this.answers = answer;
  }

  open(content: PollStats) {
    let settings = {
      ariaLabelledBy: 'modal-basic-title',
      centered: true,
      size: 'lg',
    }

    this.modalService.open(content, settings);//.result.then(result => {});
    console.log(this.poll)
    console.log(this.answers)
  }

  close(){
    this.modalService.dismissAll();
  }

  getAnswer(questionId: number): String{
    let res = this.answers.answers.find(a => a.questionId == questionId)
    if (res)
      return this.answers.answers.find(a => a.questionId == questionId).content;
    return "";
  }

  getCheckedState(questionId: number ,index: number): String{
  
   return this.getAnswer(questionId).split('/').some(a => Number(a)==index)? 'checked': ''
  }
}
