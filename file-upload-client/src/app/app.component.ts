import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <nav class="navbar navbar-dark bg-primary">
      <div class="container">
        <a class="navbar-brand" href="#">File Upload Demo</a>
      </div>
    </nav>
    <app-file-upload></app-file-upload>
  `,
  styles: []
})
export class AppComponent {
  title = 'file-upload-client';
}
