import { Component, OnInit, OnDestroy } from '@angular/core';
import { FileUploadService } from '../../services/file-upload.service';

@Component({
  selector: 'app-file-upload',
  template: `
    <div class="upload-container">
      <input type="file" (change)="onFileSelected($event)" #fileInput>
      <button (click)="uploadFile()" [disabled]="!selectedFile">Upload</button>
      <div *ngIf="uploadStatus" [class]="uploadStatus">
        {{ uploadMessage }}
      </div>
    </div>
  `,
  styles: [`
    .upload-container {
      margin: 20px;
      padding: 20px;
      border: 1px solid #ccc;
      border-radius: 4px;
    }
    .success {
      color: green;
      margin-top: 10px;
    }
    .error {
      color: red;
      margin-top: 10px;
    }
  `]
})
export class FileUploadComponent implements OnInit, OnDestroy {
  selectedFile: File | null = null;
  uploadStatus: 'success' | 'error' | null = null;
  uploadMessage: string = '';

  constructor(private fileUploadService: FileUploadService) {}

  ngOnInit() {
    this.fileUploadService.onNotification((message: string) => {
      this.uploadStatus = 'success';
      this.uploadMessage = message;
    });
  }

  ngOnDestroy() {
    // No need to unsubscribe as we're using callback
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.uploadStatus = null;
      this.uploadMessage = '';
    }
  }

  uploadFile() {
    if (this.selectedFile) {
      this.uploadStatus = null;
      this.uploadMessage = 'Uploading...';
      
      this.fileUploadService.uploadFile(this.selectedFile).subscribe({
        next: (response) => {
          this.uploadStatus = 'success';
          this.uploadMessage = 'File uploaded successfully!';
          this.selectedFile = null;
          // Reset file input
          const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
          if (fileInput) {
            fileInput.value = '';
          }
        },
        error: (error) => {
          this.uploadStatus = 'error';
          this.uploadMessage = 'Error uploading file: ' + error.message;
        }
      });
    }
  }
} 