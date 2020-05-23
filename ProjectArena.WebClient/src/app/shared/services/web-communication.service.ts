import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { ExternalResponse } from '../models/external-response.model';
import { of } from 'rxjs';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root'
})
export class WebCommunicationService {

  constructor(
    private httpClient: HttpClient,
    private loadingService: LoadingService
    ) { }

  handleError<T>(result: HttpErrorResponse) {
    if (result.status === 400) {
      return of({
        success: false,
        statusCode: result.status,
        errors: result.error ? (Object.values(result.error.errors) as string[][]).reduce((sum, next) => sum.concat(next), []) : []
      } as ExternalResponse<T>);
    } else {
      return of({
        success: false,
        statusCode: result.status,
        errors: result.error && result.error.title ? [result.error.title] : ['Unexpected error. Try again later...']
      } as ExternalResponse<T>);
    }
  }

  get<T>(url: string, params?: { [param: string]: string }, headers?: { [param: string]: string }) {
    return this.httpClient.get<T>(url,
    {
      headers,
      params
    })
    .pipe(map(result => {
      return {
        success: true,
        result
      } as ExternalResponse<T>;
    }))
    .pipe(catchError((result) => this.handleError<T>(result)));
  }

  post<Tin, Tout>(url: string, body: Tin, headers?: { [param: string]: string }) {
    return this.httpClient.post<Tout>(url, body,
      {
        headers
      })
      .pipe(map(result => {
        return {
          success: true,
          result
        } as ExternalResponse<Tout>;
      }))
      .pipe(catchError((result) => this.handleError<Tout>(result)));
  }

  put<Tin, Tout>(url: string, body: Tin, headers?: { [param: string]: string }) {
    return this.httpClient.put<Tout>(url, body,
      {
        headers
      })
      .pipe(map(result => {
        return {
          success: true,
          result
        } as ExternalResponse<Tout>;
      }))
      .pipe(catchError((result) => this.handleError<Tout>(result)));
  }

  delete<T>(url: string, params?: { [param: string]: string }, headers?: { [param: string]: string }) {
    return this.httpClient.delete<T>(url,
      {
        headers,
        params
      })
      .pipe(map(result => {
        return {
          success: true,
          result
        } as ExternalResponse<T>;
      }))
      .pipe(catchError((result) => this.handleError<T>(result)));
  }

  desync(result: ExternalResponse<unknown>) {
    if (result.statusCode >= 400 && result.statusCode < 500) {
      this.loadingService.startLoading({
        title: 'Desynchronization. Page will be refreshed in 2 seconds.'
      }, 0, true);
      setTimeout(() => {
        location.reload();
      }, 2000);
      return true;
    }
    return false;
  }
}
