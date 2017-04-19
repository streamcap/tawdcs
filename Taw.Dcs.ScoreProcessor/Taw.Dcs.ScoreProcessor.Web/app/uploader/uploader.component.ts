import { Component } from '@angular/core';

import { ApiService } from "../services/csv.service";

@Component({
    selector: 'my-uploader',
    templateUrl: "./uploader.component.html"
})

export class UploaderComponent {
    separator: string;
    csvText: string;
    isUploadingCsv: boolean;
    defaultText = "Uploaded.";

    constructor(private apiService: ApiService) {
        this.csvText = "";
        this.separator = "";
        this.isUploadingCsv = false;
    }
    
    uploadCsv(): void {
        if (!this.csvText || this.csvText === this.defaultText) {
            return;
        }
        this.isUploadingCsv = true;
        this.apiService.postCsv(this.csvText, this.separator)
            .then(() => {
                this.isUploadingCsv = false;
                this.csvText = this.defaultText;
            });
    }
}