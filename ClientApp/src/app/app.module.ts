import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';

import { AppComponent } from './components/app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';

import { LoginComponent } from './components/login/login.component'

import { SettingsComponent } from './components/settings/settings.component'
import { PollComponent } from './components/poll/poll.component'
import { PollStatisticsComponent } from './components/poll-statistics/poll-statistics.component'
import { UserInfoComponent } from './components/settings/user-info/user-info.component'

import {MatNativeDateModule} from '@angular/material/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
//import { BootstrapTabDirective } from './directives/bootstrap-tab.directive'

import * as PlotlyJS from 'plotly.js/dist/plotly.js';
import { CommonModule } from '@angular/common';
import { PlotlyModule } from 'angular-plotly.js';
PlotlyModule.plotlyjs = PlotlyJS;

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PollComponent,
    PollStatisticsComponent,
    SettingsComponent, UserInfoComponent,
    LoginComponent,

  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatNativeDateModule,
    BrowserAnimationsModule,
    CommonModule, PlotlyModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'user-login', component: LoginComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'poll', component: PollComponent },
      { path: 'poll-statistics', component: PollStatisticsComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})

export class AppModule {

}

