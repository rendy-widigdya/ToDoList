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
  loading = false;
  error = '';

  constructor(private svc: TodoListService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.svc.getAll().subscribe({
      next: (t) => {
        this.todos = t || [];
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load todos: ' + err.message;
        this.loading = false;
      },
    });
  }

  add(): void {
    const title = this.newTitle?.trim();
    if (!title) return;
    this.loading = true;
    this.error = '';
    this.svc.create(title).subscribe({
      next: (created) => {
        this.todos.push(created);
        this.newTitle = '';
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to create todo: ' + err.message;
        this.loading = false;
      },
    });
  }

  startEdit(todo: Todo): void {
    this.editingId = todo.id ?? null;
    this.editTitle = todo.title;
  }

  saveEdit(todo: Todo): void {
    const title = this.editTitle?.trim();
    if (!this.editingId || !title) return;

    this.loading = true;
    this.error = '';
    const updated: Todo = { ...todo, title };
    this.svc.update(this.editingId, updated).subscribe({
      next: () => {
        const idx = this.todos.findIndex((t) => t.id === this.editingId);
        if (idx >= 0) {
          this.todos[idx] = updated;
        }
        this.cancelEdit();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to save todo: ' + err.message;
        this.loading = false;
      },
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editTitle = '';
  }

  toggleDone(todo: Todo): void {
    if (!todo.id) return;

    this.loading = true;
    this.error = '';
    const updated: Todo = { ...todo, isDone: !todo.isDone };
    this.svc.update(todo.id, updated).subscribe({
      next: () => {
        const idx = this.todos.findIndex((t) => t.id === todo.id);
        if (idx >= 0) {
          this.todos[idx] = updated;
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to toggle todo: ' + err.message;
        this.loading = false;
      },
    });
  }

  delete(todo: Todo): void {
    if (!todo.id) return;
    this.loading = true;
    this.error = '';
    this.svc.delete(todo.id).subscribe({
      next: () => {
        this.todos = this.todos.filter((t) => t.id !== todo.id);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to delete todo: ' + err.message;
        this.loading = false;
      },
    });
  }
}
