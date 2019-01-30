import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TaskStatusCenterComponent } from './task-status-center.component';
import { TaskStatusMasterComponent } from './task-status-master/task-status-master.component';

const routes: Routes = [{
  path: "",
  component: TaskStatusCenterComponent,
  children: [
    {
      path: ":key",
      component: TaskStatusMasterComponent,
    },
    {
      path: "",
      component: TaskStatusMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TaskStatusRoutingModule { }
