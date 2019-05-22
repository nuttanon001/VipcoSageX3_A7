import { Component, OnInit, ViewContainerRef, ViewChild } from '@angular/core';
import { BaseMasterComponent } from 'src/app/shared2/baseclases/base-master-component';
import { PurchaseExtend } from '../shared/purchase-extend.model';
import { PurchaseExtendService } from '../shared/purchase-extend.service';
import { PurchaseExtendCommunicateService } from '../shared/purchase-extend-communicate.service';
import { AuthService } from 'src/app/core/auth/auth.service';
import { DialogsService } from 'src/app/dialogs/shared/dialogs.service';
import { PurchaseExtendTableComponent } from '../purchase-extend-table/purchase-extend-table.component';

@Component({
  selector: 'app-purchase-extend-master',
  templateUrl: './purchase-extend-master.component.html',
  styleUrls: ['./purchase-extend-master.component.scss'],
  providers: [PurchaseExtendCommunicateService]
})
export class PurchaseExtendMasterComponent
  extends BaseMasterComponent<PurchaseExtend, PurchaseExtendService, PurchaseExtendCommunicateService> {
  constructor(
    service: PurchaseExtendService,
    serviceCom: PurchaseExtendCommunicateService,
    serviceAuth: AuthService,
    serviceDialog: DialogsService,
    viewCon: ViewContainerRef,
  ) {
    super(service, serviceCom, serviceAuth, serviceDialog, viewCon);
  }

  backToSchedule: boolean = false;
  showReport?: boolean = false;
  @ViewChild(PurchaseExtendTableComponent)
  private tableComponent: PurchaseExtendTableComponent;

  onReloadData(): void {
    this.tableComponent.reloadData();
  }

  onReportClick(): void {
    this.dialogsService.dialogDatePicker(this.viewContainerRef, 1)
      .subscribe(datePicker => {
        if (datePicker) {
          this.onLoading = true;
          this.service.getXlsx({SDate:datePicker.SDate,EDate:datePicker.EDate}).subscribe(data => {
            // console.log(data);
            this.onLoading = false;
          }, () => this.onLoading = false, () => this.onLoading = false);
        }
      });
  }
}
