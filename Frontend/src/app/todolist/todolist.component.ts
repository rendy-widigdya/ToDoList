import { Component, DestroyRef, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { catchError, of } from 'rxjs';
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
  todos = signal<Todo[]>([]);
  newTitle = '';
  editingId: string | null = null;
  editTitle = '';
  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor(
    private readonly todoService: TodoListService,
    private readonly destroyRef: DestroyRef,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.todoService
      .getAll()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError((err) => {
          this.error.set('Failed to load todos. Please try again.');
          console.error('Error loading todos:', err);
          return of([]);
        }),
      )
      .subscribe((todos) => {
        this.todos.set(todos || []);
        this.isLoading.set(false);
      });
  }

  add(): void {
    const title = this.newTitle?.trim();
    if (!title) return;

    this.isLoading.set(true);
    this.error.set(null);
    this.todoService
      .create(title)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError((err) => {
          this.error.set('Failed to create todo. Please try again.');
          console.error('Error creating todo:', err);
          return of(null);
        }),
      )
      .subscribe((created) => {
        if (created) {
          this.todos.update((todos) => [...todos, created]);
          this.newTitle = '';
        }
        this.isLoading.set(false);
      });
  }

  startEdit(todo: Todo): void {
    this.editingId = todo.id;
    this.editTitle = todo.title;
  }

  saveEdit(todo: Todo): void {
    if (!this.editingId) return;

    const title = this.editTitle.trim();
    if (!title) {
      this.cancelEdit();
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);
    const updated: Todo = { ...todo, title };
    this.todoService
      .update(this.editingId, updated)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError((err) => {
          this.error.set('Failed to update todo. Please try again.');
          console.error('Error updating todo:', err);
          return of(null);
        }),
      )
      .subscribe((result) => {
        if (result) {
          this.todos.update((todos) =>
            todos.map((t) => (t.id === this.editingId ? updated : t)),
          );
          this.editingId = null;
          this.editTitle = '';
        }
        this.isLoading.set(false);
      });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editTitle = '';
  }

  toggleDone(todo: Todo): void {
    const updated: Todo = { ...todo, isDone: !todo.isDone };
    this.todoService
      .update(todo.id, updated)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError((err) => {
          this.error.set('Failed to update todo. Please try again.');
          console.error('Error updating todo:', err);
          return of(null);
        }),
      )
      .subscribe((result) => {
        if (result) {
          this.todos.update((todos) =>
            todos.map((t) => (t.id === todo.id ? updated : t)),
          );
        }
      });
  }

  delete(todo: Todo): void {
    if (!todo.id) return;

    this.isLoading.set(true);
    this.error.set(null);
    this.todoService
      .delete(todo.id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError((err) => {
          this.error.set('Failed to delete todo. Please try again.');
          console.error('Error deleting todo:', err);
          return of(null);
        }),
      )
      .subscribe((result) => {
        if (result !== null) {
          this.todos.update((todos) => todos.filter((t) => t.id !== todo.id));
        }
        this.isLoading.set(false);
      });
  }

  trackByTodoId(_index: number, todo: Todo): string {
    return todo.id;
  }
}
