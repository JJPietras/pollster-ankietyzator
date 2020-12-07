
import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-select-poll',
  templateUrl: './select-poll.component.html',
  styleUrls: ['./select-poll.component.scss']
})

export class SelectPollComponent{

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute) {

  }

  private pollId: number;
  polls: Poll[];

  ngOnInit() {
    this.pollId = Number(this.route.snapshot.paramMap.get('id'));
    console.log(this.pollId);

    this.http.get<Request>(this.baseUrl + 'polls/get-user-un-filled').subscribe(result => {
      this.polls = result.data;
      console.log(this.polls)
    }, error => console.error(error));
  }

}
