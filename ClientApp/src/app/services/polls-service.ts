import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: "root",
})
export class PollsService {
    pollSource: BehaviorSubject<Poll>;
    currentPoll: Observable<Poll>;

    changePoll(poll: Poll) {
        if (!this.pollSource){
            this.pollSource = new BehaviorSubject(poll);
            this.currentPoll = this.pollSource.asObservable();
        }
        this.pollSource.next(poll);
    }
}
