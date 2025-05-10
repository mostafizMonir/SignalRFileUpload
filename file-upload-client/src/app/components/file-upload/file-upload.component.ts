import { Component, OnInit, OnDestroy } from '@angular/core';
import { FileUploadService } from '../../services/file-upload.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-file-upload',
  template: `
    <div class="container mt-5">
      <div class="card">
        <div class="card-header">
          <h3>File Upload</h3>
        </div>
        <div class="card-body">
          <div class="mb-3">
            <input type="file" class="form-control" (change)="onFileSelected($event)">
          </div>
          <button class="btn btn-primary" (click)="onUpload()" [disabled]="!selectedFile">
            Upload
          </button>

          <div class="mt-4">
            <h4>Notifications</h4>
            <div class="list-group">
              <div *ngFor="let notification of notifications" 
                   class="list-group-item"
                   [ngClass]="{'list-group-item-success': notification.success, 
                             'list-group-item-danger': !notification.success}">
                <h5 class="mb-1">{{notification.fileName}}</h5>
                <p class="mb-1">{{notification.message}}</p>
                <small>Processed at: {{notification.processedAt | date:'medium'}}</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class FileUploadComponent implements OnInit, OnDestroy {
  selectedFile: File | null = null;
  notifications: any[] = [];
  private subscription: Subscription;

  constructor(private fileUploadService: FileUploadService) {
    this.subscription = this.fileUploadService.getFileProcessedNotifications()
      .subscribe(notification => {
        this.notifications.unshift(notification);
      });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  onUpload(): void {
    if (this.selectedFile) {
      this.fileUploadService.uploadFile(this.selectedFile)
        .subscribe({
          next: (response) => {
            console.log('File uploaded successfully', response);
          },
          error: (error) => {
            console.error('Error uploading file', error);
          }
        });
    }
  }
} 