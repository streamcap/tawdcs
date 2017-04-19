import { Component, Input } from '@angular/core';

import { ApiService } from "../services/csv.service";
import { ScoreEvent } from '../models/ScoreEvent';
import { TeamData } from "../models/TeamData";

@Component({
    selector: "my-presenter",
    templateUrl: "./presenter.component.html"
})
export class PresenterComponent {
    events: ScoreEvent[];
    summary: string[];
    teamDataCollection: TeamData[];
    hasEvents: boolean;
    selectedTeam: any;
    selectedTab: any;
    isLoading: boolean;

    private selectedgame: string;
    private selectedRule: number;

    @Input()
    set rule(rule: number) {
        this.selectedRule = rule;
    }

    @Input()
    set game(game: string) {
        this.selectedgame = (game && game.trim()) || null;
        this.selectedTeam = null;
        this.selectedTab = null;
        this.isLoading = false;
        this.downloadSummary();
    }

    get name(): string { return this.selectedgame; }

    get rule(): number { return this.selectedRule; }

    constructor(private apiService: ApiService) {
        this.selectedTeam = null;
        //this.selectedRule = null;
        this.selectedTab = null;
        this.isLoading = false;
    }

    downloadSummary(): void {
        this.isLoading = true;
        this.resetLists();        
        this.apiService.getSummary(this.selectedgame)
            .then((lines: string[]) => {
                this.summary = lines;
                this.isLoading = false;
            });
    }

    downloadTeamData(): void {
        this.isLoading = true;
        this.resetLists();
        this.apiService.getTeamData(this.selectedgame)
            .then((teamData: TeamData[]) => {
                this.teamDataCollection = teamData;
                this.selectedTeam = null;
                this.selectedTab = null;
                this.isLoading = false;
            });
    }

    resetLists(): void {
        this.events = [];
        this.summary = [];
        this.teamDataCollection = null;
    }

    selectTeam(team: any): void {
        this.selectedTeam = team;
        if (team.tabs && team.tabs.length > 0) {
            this.selectedTab = team.tabs[0];
        }
    }

    selectTab(tab: any): void {
        this.selectedTab = tab;
    }

}