import { Component } from '@angular/core';
import { OderNotificationComponent } from './oder-notification/oder-notification.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [OderNotificationComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'order-notifications';
}
