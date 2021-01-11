import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { Pipe, PipeTransform } from '@angular/core';

import { AppComponent } from './components/app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';

import { LoginComponent } from './components/login/login.component'

import { SettingsComponent } from './components/settings/settings.component'
import { PollComponent } from './components/poll/poll.component'
import { SelectPollComponent } from './components/select-poll/select-poll.component'
import { PollStatisticsComponent } from './components/poll-statistics/poll-statistics.component'
import { PollsStatisticsComponent } from './components/polls-statistics/polls-statistics.component'
import { UserInfoComponent } from './components/settings/user-info/user-info.component'
import { AdminInfoComponent } from './components/settings/admin-info/admin-info.component'
import { PollCreatorComponent } from './components/poll-creator/poll-creator.component'
import { PollAnswersComponent } from './components/poll-answers/poll-answers.component'
import { PollsAdminPanelComponent } from './components/polls-admin-panel/polls-admin-panel.component'

import { PollsAdminPanelPipe } from './pipes/polls-admin-panel-filter.pipe'

import {MatNativeDateModule} from '@angular/material/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
//import { BootstrapTabDirective } from './directives/bootstrap-tab.directive'


import * as PlotlyJS from 'plotly.js/dist/plotly.js';
import { CommonModule } from '@angular/common';
import { PlotlyModule } from 'angular-plotly.js';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatFormFieldModule, MatInput, MatInputModule, MatSelect, MatSelectModule } from '@angular/material';


PlotlyModule.plotlyjs = PlotlyJS;

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PollComponent,
    SelectPollComponent,
    PollStatisticsComponent, PollsStatisticsComponent,
    SettingsComponent, UserInfoComponent,
    LoginComponent,
    AdminInfoComponent,
    PollCreatorComponent,
    PollAnswersComponent,
    PollsAdminPanelComponent,
    PollsAdminPanelPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    BrowserAnimationsModule,
    CommonModule, PlotlyModule,
    NgbModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'user-login', component: LoginComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'poll', component: PollComponent },
      { path: 'select-poll', component: SelectPollComponent },
      { path: 'poll-statistics', component: PollsStatisticsComponent },
      { path: 'poll-statistics/:id', component: PollStatisticsComponent },
      { path: 'poll-creator', component: PollCreatorComponent },
      { path: 'poll-answers', component: PollAnswersComponent },
      { path: 'polls-admin', component: PollsAdminPanelComponent }
      
    ]),
  ],
  providers: [],
  bootstrap: [AppComponent]
})

export class AppModule {

}

