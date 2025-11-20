import { NgModule } from '@angular/core';
import { TodoListComponent } from './todolist.component';
import { TodoListRoutingModule } from './todolist-routing.module';

@NgModule({
  imports: [TodoListComponent, TodoListRoutingModule],
})
export class TodoListModule {}
