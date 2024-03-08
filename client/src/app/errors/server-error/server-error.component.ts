import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css'],
})
export class ServerErrorComponent {
  error: any;

  constructor(private router: Router) {
    // Get the navigation extras plugged in via the interceptor.
    const navigation = this.router.getCurrentNavigation();
    // retrieve the custom data defined in the interceptor.
    this.error = navigation?.extras?.state?.['error'];
  }
}
