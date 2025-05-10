import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private hubConnection: HubConnection;

  constructor(private http: HttpClient) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/notificationHub')
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('SignalR Connection Error: ', err));
  }

  uploadFile(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post('/api/FileUpload', formData);
  }

  onNotification(callback: (message: string) => void): void {
    this.hubConnection.on('ReceiveNotification', (message: string) => {
      callback(message);
    });
  }
} 
 