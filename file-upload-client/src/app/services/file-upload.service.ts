import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private hubConnection: signalR.HubConnection;
  private fileProcessedSubject = new Subject<any>();

  constructor(private http: HttpClient) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/notificationHub')
      .build();

    this.hubConnection.on('FileProcessed', (notification) => {
      this.fileProcessedSubject.next(notification);
    });

    this.startConnection();
  }

  private async startConnection() {
    try {
      await this.hubConnection.start();
      console.log('SignalR connection started');
    } catch (err) {
      console.error('Error starting SignalR connection:', err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  uploadFile(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post('/api/FileUpload', formData);
  }

  getFileProcessedNotifications(): Observable<any> {
    return this.fileProcessedSubject.asObservable();
  }
} 
 