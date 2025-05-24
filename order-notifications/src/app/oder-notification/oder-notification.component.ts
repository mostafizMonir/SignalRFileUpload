import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-oder-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './oder-notification.component.html',
  styleUrl: './oder-notification.component.css'
})
export class OderNotificationComponent implements OnInit, OnDestroy {
  orders: any[] = [];
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:7002/orderHub', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();
  }

  ngOnInit() {
    this.startHubConnection();
    this.getOrders();
  }

  ngOnDestroy() {
    this.hubConnection.stop();
  }

  private startHubConnection() {
    this.hubConnection.start()
      .then(() => {
        console.log('Connected to SignalR Hub');
        
        // Listen for order updates
        this.hubConnection.on('OrderCreated', (order: any) => {
          console.log('Signal R .....New order received:', order);
          this.orders = [...this.orders, order];
        });

        this.hubConnection.on('OrderStatusUpdated', (order: any) => {
          console.log('Signal R .....Order updated:', order);
          const index = this.orders.findIndex(o => o.id === order.id);
          if (index !== -1) {
            this.orders[index] = order;
            this.orders = [...this.orders];
          }
        });
      })
      .catch(err => console.error('Error while starting SignalR connection:', err));
  }

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
      // The order will be added to the list through SignalR
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
