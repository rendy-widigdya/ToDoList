import { NgModule } from '@angular/core';
import { TodoListComponent } from './todolist.component';
import { TodoListRoutingModule } from './todolist-routing.module';
import { TodoListService } from './todolist.service';

@NgModule({
  imports: [TodoListComponent, TodoListRoutingModule],
  providers: [TodoListService],
})
export class TodoListModule {}
