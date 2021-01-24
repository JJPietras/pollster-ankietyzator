import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { Pipe, PipeTransform } from '@angular/core';
import { MatDialogModule } from '@angular/material';

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

import { PollCreatorComponent } from './components/poll-creator/poll-creator.component'
import { PollAnswersComponent } from './components/poll-answers/poll-answers.component'
import { PollsAdminPanelComponent } from './components/polls-admin-panel/polls-admin-panel.component'

import { FooterComponent } from './components/footer/footer.component'

import { PollsAdminPanelPipe } from './pipes/polls-admin-panel-filter.pipe'

import { KeysAdminPipe } from './pipes/admin-keys-filter.pipe'
import { UsersAdminPipe } from './pipes/admin-users-filter.pipe'

import {MatNativeDateModule} from '@angular/material/core';

import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { ClonePollsComponent } from './components/settings/clone-polls/clone-polls.component'

import * as PlotlyJS from 'plotly.js/dist/plotly.js';
import { CommonModule } from '@angular/common';
import { PlotlyModule } from 'angular-plotly.js';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatAutocompleteModule, MatFormFieldModule, MatInput, MatInputModule, MatSelect, MatSelectModule } from '@angular/material';

import { AdminEditKeyComponent } from './components/settings/admin-keys/admin-edit-key/admin-edit-key.component';
import { AdminKeysComponent } from './components/settings/admin-keys/admin-keys.component'

import { AdminUsersComponent } from './components/settings/admin-users/admin-users.component';
import { AdminEditUserComponent } from './components/settings/admin-users/admin-edit-user/admin-edit-user.component';

import { NgxSliderModule } from '@angular-slider/ngx-slider';

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
    AdminKeysComponent, AdminUsersComponent,
    AdminEditKeyComponent, AdminEditUserComponent,
    PollCreatorComponent,
    PollAnswersComponent,
    PollsAdminPanelComponent,
    PollsAdminPanelPipe, KeysAdminPipe, UsersAdminPipe,
    FooterComponent,
    ClonePollsComponent
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
    MatDialogModule,
    MatSelectModule,
    MatAutocompleteModule,
    BrowserAnimationsModule,
    CommonModule, PlotlyModule,
    NgbModule,
    NgxSliderModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'user-login', component: LoginComponent },
      { path: 'settings', component: SettingsComponent },
      { path: 'poll', component: PollComponent },
      { path: 'select-poll', component: SelectPollComponent },
      { path: 'poll-statistics', component: PollsStatisticsComponent },
      { path: 'poll-statistics/:id', component: PollStatisticsComponent },
      { path: 'poll-creator/:type', component: PollCreatorComponent },
      { path: 'poll-answers', component: PollAnswersComponent },
      { path: 'polls-admin', component: PollsAdminPanelComponent }
      
    ], {
      scrollPositionRestoration: 'enabled',
    }),
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [AdminEditKeyComponent, AdminEditUserComponent]
})

export class AppModule {

}

