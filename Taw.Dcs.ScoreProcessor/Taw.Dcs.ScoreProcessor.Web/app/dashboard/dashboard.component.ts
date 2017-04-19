import { Component } from '@angular/core';

import { ApiService } from "../services/csv.service";

@Component({
    selector: 'my-dashboard',
    templateUrl: "./dashboard.component.html"
})

export class DashboardComponent {

    inputData: boolean;
    
    constructor(private apiService: ApiService) {
    }
    
    clearEvents(): void {
        this.apiService.clearServerCache("events")
            .then(() => {
            this.inputData = true;
        });
    }

}