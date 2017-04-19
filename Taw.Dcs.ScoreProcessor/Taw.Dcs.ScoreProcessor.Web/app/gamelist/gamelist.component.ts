import { Component } from '@angular/core';
import { ApiService } from "../services/csv.service";
import { KillScoreRule } from "../models/KillScoreRule"

@Component({
    selector: "my-gamelist",
    templateUrl: "./gamelist.component.html"
})

export class GameListComponent {

    games: string[];
    isLoading: boolean;
    rules: KillScoreRule[];
    selectedGame: string;
    selectedRule: number;

    constructor(private apiService: ApiService) {
        this.selectedGame = null;
        this.isLoading = true;
        this.rules = this.getKillScoreRules();
        this.selectedRule = 2;
        console.log("Getting games...");
        apiService.getGamesList().then(list => {
            console.log("Got " + list.length + " games...");
            this.games = list;
            this.isLoading = false;
        });
    }

    presentGame(game: string) {
        console.log("Clicked button for " + game + ":" + this.selectedRule.toString());
        this.selectedGame = game;
    }

    getKillScoreRules(): KillScoreRule[] {
        return [
            new KillScoreRule(0, "Do nothing"),
            new KillScoreRule(1, "Last only"),
            new KillScoreRule(2, "Share alike")
        ];
    }
}