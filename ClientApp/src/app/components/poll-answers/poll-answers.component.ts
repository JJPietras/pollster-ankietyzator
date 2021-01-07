
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

  ngOnInit() {

  }

  @Input()
  set content(content: PollStats) {
    this.poll = content;
  }

  open(content: PollStats) {
    let settings = {
      ariaLabelledBy: 'modal-basic-title',
      centered: true,
    }

    this.modalService.open(content, settings).result.then(result => {})

  }


}
