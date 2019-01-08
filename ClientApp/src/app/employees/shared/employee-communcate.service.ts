import { Injectable } from '@angular/core';
import { BaseCommunicateService } from '../../shared/base-communicate.service';
import { Employee } from './employee.model';

@Injectable()
export class EmployeeCommuncateService extends BaseCommunicateService<Employee> {
  constructor() {
    super();
  }
}
