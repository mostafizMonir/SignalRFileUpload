import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-oder-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './oder-notification.component.html',
  styleUrl: './oder-notification.component.css'
})
export class OderNotificationComponent {
  orders: any[] = [];

  makeOrder() {
    console.log('Making order...');
    fetch('http://localhost:7002/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({})
    })
    .then(response => response.json())
    .then(data => {
      console.log('Order created successfully:', data);
      this.getOrders(); // Refresh orders after creating new one
    })
    .catch(error => console.error('Error creating order:', error));
  }

  getOrders() {
    console.log('Fetching orders...');
    fetch('http://localhost:7002/orders', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    .then(response => response.json())
    .then(data => {
      console.log('Orders retrieved successfully:', data);
      this.orders = data;
    })
    .catch(error => console.error('Error fetching orders:', error));
  }
}
