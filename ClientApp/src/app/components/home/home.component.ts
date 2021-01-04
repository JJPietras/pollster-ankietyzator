import { animate, style, transition, trigger } from '@angular/animations';
import { Component } from '@angular/core';

/*
var myIndex = 0;
function carousel() {
  var i;
  var x = document.getElementsByClassName("mySlides");
  for (i = 0; i < x.length; i++) {
    x[i].style.display = "none";  
  }
  myIndex++;
  if (myIndex > x.length) {myIndex = 1}    
  x[myIndex-1].style.display = "block";  
  setTimeout(carousel, 4000);    
}*/


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
  
})
export class HomeComponent {


  constructor(){
    //carousel();
  }


  
}
