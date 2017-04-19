import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from "@angular/forms";
import { HttpModule, JsonpModule } from "@angular/http";
import { RouterModule } from '@angular/router';

import { AppComponent } from './app/app.component';
import { DashboardComponent } from "./dashboard/dashboard.component";
import { PresenterComponent } from "./presenter/presenter.component";
import { GameListComponent } from "./gamelist/gamelist.component";
import { UploaderComponent } from "./uploader/uploader.component";

import { ApiService } from "./services/csv.service";

@NgModule({
    imports: [BrowserModule, HttpModule, FormsModule, RouterModule, JsonpModule],
    declarations: [
        AppComponent,
        DashboardComponent,
        PresenterComponent,
        GameListComponent,
        UploaderComponent
    ],
    providers: [ApiService],
    bootstrap: [AppComponent]
})
export class AppModule { }
