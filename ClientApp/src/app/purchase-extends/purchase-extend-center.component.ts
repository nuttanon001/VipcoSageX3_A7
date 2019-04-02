import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { AuthService } from '../core/auth/auth.service';
import { User } from '../users/shared/user.model';
import { DialogsService } from '../dialogs/shared/dialogs.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-purchase-extend-center',
  template: `<router-outlet></router-outlet>`,
  styles: []
})
export class PurchaseExtendCenterComponent implements OnInit {
  constructor(
    private serviceAuth: AuthService,
    private serviceDialogs: DialogsService,
    private viewCon: ViewContainerRef,
    private router: Router
  ) {
    this.serviceAuth.currentUser.subscribe(x => {
      // console.log(JSON.stringify(x));
      this.currentUser = x;
    });
  }

  currentUser: User;

  ngOnInit() {
    // console.log(JSON.stringify(this.currentUser));

    if (!this.currentUser || this.currentUser.LevelUser < 2) {
      if (!this.currentUser || !this.currentUser.SubLevel || this.currentUser.SubLevel !== 2) {
        this.serviceDialogs.error("Waining Message", "Access is restricted. please contact administrator !!!", this.viewCon).
          subscribe(() => this.router.navigate(["login"]));
      }
    }
  }
}
