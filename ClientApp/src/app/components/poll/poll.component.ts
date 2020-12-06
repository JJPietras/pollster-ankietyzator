
import {Component, OnInit, OnDestroy, Input, Inject, EventEmitter, Output} from '@angular/core';
import { UserLogin } from '../../models/user-login.model';
import { HttpClient } from '@angular/common/http';
import {Router, ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-poll',
  templateUrl: './poll.component.html',
  styleUrls: ['./poll.component.scss']
})

export class PollComponent{

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router, private route: ActivatedRoute) {

  }

  ngOnInit() {
    const heroId = this.route.snapshot.paramMap.get('id');

    console.log(heroId);
  }

}
