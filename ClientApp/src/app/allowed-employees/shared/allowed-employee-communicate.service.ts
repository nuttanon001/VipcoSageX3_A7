import { Injectable } from '@angular/core';
import { AllowedEmployee } from './allowed-employee.model';
import { BaseCommunicateService } from 'src/app/shared2/baseclases/base-communicate.service';

@Injectable()
export class AllowedEmployeeCommunicateService extends BaseCommunicateService<AllowedEmployee> {
  constructor() { super(); }
}
