import "rxjs/add/observable/throw";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";

import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { CaptchaApiResponse } from "./dnt-captcha-api-response";
import { CaptchaLanguage } from "./dnt-captcha-language";

@Component({
  selector: "dnt-captcha",
  templateUrl: "./dnt-captcha.component.html",
  styleUrls: ["./dnt-captcha.component.css"],
})
export class CaptchaComponent implements OnInit {
  apiResponse = new CaptchaApiResponse();
  hiddenInputName = "CaptchaText";
  hiddenTokenName = "CaptchaToken";
  inputName = "CaptchaInputText";

  @Input() text: string;
  @Output() textChange = new EventEmitter<string>();

  @Input() token: string;
  @Output() tokenChange = new EventEmitter<string>();

  @Input() inputText: string;
  @Output() inputTextChange = new EventEmitter<string>();

  @Input() placeholder: string;
  @Input() apiUrl: string;
  @Input() backColor: string;
  @Input() fontName: string;
  @Input() fontSize: number;
  @Input() foreColor: string;
  @Input() language: CaptchaLanguage;
  @Input() max: number;
  @Input() min: number;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.doRefresh();
  }

  private handleError(error: HttpErrorResponse): Observable<any> {
    console.error("getCaptchaInfo error: ", error);
    return Observable.throw(error.statusText);
  }

  getCaptchaInfo(): Observable<CaptchaApiResponse> {
    return this.http
      .get<CaptchaApiResponse>(`${this.apiUrl}`, {
        withCredentials: true /* For CORS */,
      })
      .map((response) => response || {})
      .catch(this.handleError);
  }

  doRefresh() {
    this.inputText = "";
    this.getCaptchaInfo().subscribe((data) => {
      this.apiResponse = data;
      this.text = data.CaptchaTextValue;
      this.onTextChange();
      this.token = data.dntCaptchaTokenValue;
      this.onTokenChange();
    });
  }

  onTextChange() {
    this.textChange.emit(this.text);
  }

  onTokenChange() {
    this.tokenChange.emit(this.token);
  }

  onInputTextChange() {
    this.inputTextChange.emit(this.inputText);
  }
}
