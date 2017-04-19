import { Injectable } from "@angular/core";
import { Headers, Http, Response } from "@angular/http";

import 'rxjs/add/operator/toPromise';

import { TeamData } from "../models/TeamData";

@Injectable()
export class ApiService {

    private apiUrl = "/api";
    private headers = new Headers({ "Content-type": "application/json" });

    constructor(private http: Http) { }

    postCsv(csv: string, separator: string): Promise<boolean> {
        return this.http.post(this.apiUrl + "/Csv", JSON.stringify({ csv, separator }), { headers: this.headers })
            .toPromise()
            .then(() => { return true; })
            .catch((error: any) => {
                this.handleError(error);
                return false;
            });
    }

    getSummary(game: string): Promise<string[]> {
        console.log(`Summary game for server: ${game}, stringified to ${JSON.stringify(game)}`);
        return this.http.post(this.apiUrl + "/Summary/", JSON.stringify(game), { headers: this.headers })
            .toPromise()
            .then((result: Response) => {
                var json = result.json();
                var parsed = json as string[];
                return parsed;
            });
    }

    getTeamData(game: string): Promise<TeamData[]> {
        console.log(`TeamData game for server: ${game}, stringified to ${JSON.stringify(game)}`);
        return this.http.post(this.apiUrl + "/TeamData/", JSON.stringify(game), { headers: this.headers })
            .toPromise()
            .then((result: Response) => {
                var json = result.json();                
                return json as TeamData[];
            })
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    }

    clearServerCache(tableName: string): Promise<Response> {
        return this.http.post(this.apiUrl + "/ClearCache", JSON.stringify(tableName), { headers: this.headers })
            .toPromise();
    }

    getGamesList(): Promise<string[]> {
        return this.http.get(this.apiUrl + "/GameNames", { headers: this.headers })
            .toPromise()
            .then((result: Response) => {
                return result.json() as string[];
            });
    }
}