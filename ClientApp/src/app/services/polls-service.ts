import { Injectable, OnInit, Inject } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';

@Injectable({
  providedIn: "root",
})
export class PollsService {
  pollSource: BehaviorSubject<Poll>;
  currentPoll: Observable<Poll>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router) { 

  }

  changePoll(poll: Poll) {
    if (!this.pollSource) {
      this.pollSource = new BehaviorSubject(poll);
      this.currentPoll = this.pollSource.asObservable();
    }
    this.pollSource.next(poll);
  }

  pollStatsSource: BehaviorSubject<PollStats>;
  currentPollStats: Observable<PollStats>;


  changePollStats(poll: PollStats) {
    if (!this.pollStatsSource) {
      this.pollStatsSource = new BehaviorSubject(poll);
      this.currentPollStats = this.pollStatsSource.asObservable();
    }
    this.pollStatsSource.next(poll);
  }

  newPollSource: BehaviorSubject<NewPoll>;
  currentNewPoll: Observable<NewPoll>;

  changeNewPoll(poll: NewPoll) {
    if (!this.newPollSource) {
      this.newPollSource = new BehaviorSubject(poll);
      this.currentNewPoll = this.newPollSource.asObservable();
    }
    this.newPollSource.next(poll);
  }

  deletePoll(poll: PollStats) {
    Swal.fire({
      showDenyButton: true,
      title: `czy napewno chcesz usunąć ankietę ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
      .then(
        (result) => {
          if (result.isConfirmed) {
            this.showLoading('Usuwanie ankiety.')

            this.http.delete(this.baseUrl + 'polls/remove-poll/' + poll.pollId).subscribe(result => {
              console.log(result);
              this.router.navigate(['/poll-statistics']);
              Swal.close();
            }, error => {
              console.log(error);
              Swal.close();
              Swal.fire("Błąd", error.message, "error");
            });
          }
        }
      );
  }

  archivePoll(poll: PollStats) {
    Swal.fire({
      showDenyButton: true,
      title: `Zarchiwizować ankietę ? `,
      confirmButtonText: `Tak`,
      denyButtonText: `Nie`,
    })
      .then(
        (result) => {
          if (result.isConfirmed) {
            this.showLoading('Archiwizacja ankiety.')

            this.http.put(this.baseUrl + 'polls/close-poll/' + poll.pollId, poll).subscribe(result => {
              console.log(result);
              this.router.navigate(['/poll-statistics']);
              Swal.close();
            }, error => {
              console.log(error);
              Swal.close();
              Swal.fire("Błąd", error.message, "error");
            });
          }
        }
      );
  }

  showLoading(message: string) {

    let timerInterval;
    Swal.fire({
      title: message,
      timer: 1000,
      timerProgressBar: true,
      didOpen: () => {
        Swal.showLoading()
        timerInterval = setInterval(() => { }, 100)
      },
      willClose: () => {
        clearInterval(timerInterval)
      }
    }).then((result) => {
      if (result.dismiss === Swal.DismissReason.timer) {
      }
    })
  }

}
