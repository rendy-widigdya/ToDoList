import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoListService } from './todolist.service';
import { Todo } from './todo.model';

@Component({
  selector: 'app-todolist',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todolist.component.html',
  styleUrls: ['./todolist.component.scss'],
})
export class TodoListComponent implements OnInit {
  todos: Todo[] = [];
  newTitle = '';
  editingId: string | null = null;
  editTitle = '';

  constructor(private svc: TodoListService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.svc.getAll().subscribe({
      next: (t) => (this.todos = t || []),
      error: (err) => console.error('Failed to load todos', err),
    });
  }

  add(): void {
    const title = this.newTitle?.trim();
    if (!title) return;
    this.svc.create(title).subscribe({
      next: (created) => {
        this.todos.push(created);
        this.newTitle = '';
      },
      error: (err) => console.error('Failed to create todo', err),
    });
  }

  startEdit(todo: Todo): void {
    this.editingId = todo.id ?? null;
    this.editTitle = todo.title;
  }

  saveEdit(todo: Todo): void {
    const title = this.editTitle?.trim();
    if (!this.editingId || !title) return;

    const updated: Todo = { ...todo, title };
    this.svc.update(this.editingId, updated).subscribe({
      next: () => {
        const idx = this.todos.findIndex((t) => t.id === this.editingId);
        if (idx >= 0) {
          this.todos[idx] = updated;
        }
        this.cancelEdit();
      },
      error: (err) => console.error('Failed to save todo', err),
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editTitle = '';
  }

  toggleDone(todo: Todo): void {
    if (!todo.id) return;

    const updated: Todo = { ...todo, isDone: !todo.isDone };
    this.svc.update(todo.id, updated).subscribe({
      next: () => {
        const idx = this.todos.findIndex((t) => t.id === todo.id);
        if (idx >= 0) {
          this.todos[idx] = updated;
        }
      },
      error: (err) => console.error('Failed to toggle todo', err),
    });
  }

  delete(todo: Todo): void {
    if (!todo.id) return;
    this.svc.delete(todo.id).subscribe({
      next: () => {
        this.todos = this.todos.filter((t) => t.id !== todo.id);
      },
      error: (err) => console.error('Failed to delete todo', err),
    });
  }
}
