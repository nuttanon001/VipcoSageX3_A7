import { Component } from '@angular/core';
import { MessageService } from '../../shared/message.service';
import {
  trigger, state, style, animate, transition
} from '@angular/animations';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  animations: [
    trigger('shrinkOut', [
      state('in', style({ height: '*' })),
      transition('* => void', [
        style({ height: '*' }),
        animate(250, style({ height: 0 }))
      ]),
      transition('void => *', [
        style({ height: 0 }),
        animate(250, style({ height: '*' }))
      ]),
    ])
  ]
})
export class AppComponent {
  title = 'app';
  lastError: string[];

  constructor(private serviceMessage: MessageService) {
    this.serviceMessage.messages.subscribe(error => {
      //debug here
      // console.log("Error");
      this.lastError = error.slice();
      setTimeout(() => this.clearError(), 10000);
    });
  }

  get error(): string[] {
    return this.lastError;
  }
  clearError() {
    if (this.lastError) {
      this.lastError = null;
      this.serviceMessage.clear();
    }
  }
}
