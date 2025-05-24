import { Component } from '@angular/core';

@Component({
  selector: 'app-oder-notification',
  imports: [],
  templateUrl: './oder-notification.component.html',
  styleUrl: './oder-notification.component.css'
})
export class OderNotificationComponent {

  makeOrder() {
    console.log('Order made');
    fetch('/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        orderId: Date.now()
      })
    })
    .then(response => response.json())
    .catch(error => console.error('Error:', error));
  }
}
