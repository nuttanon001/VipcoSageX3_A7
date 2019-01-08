import { Injectable } from '@angular/core';
// import { Workgroup } from "../work-groups/shared/workgroup.model";
import { Observable, Subject } from 'rxjs';

@Injectable()
export class ShareService {
  private shardData = new Subject<any>();
  constructor() {
    //this.WorkGroup = {
    //  WorkGroupId: 1
    //};
  }

  // Get SharedData
  getSharedData(): Observable<any> {
    return this.shardData.asObservable();
  }
  // Set WorkGroup
  setSharedData(_shardData: any): void {
    this.shardData.next(_shardData);
  }
}
