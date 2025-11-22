import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo } from './todo.model';
import { environment } from '../../environments/environment';

@Injectable()
export class TodoListService {
  private readonly base = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  public getAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.base);
  }

  public create(title: string): Observable<Todo> {
    return this.http.post<Todo>(this.base, { title });
  }

  public update(id: string, todo: Todo): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}`, todo);
  }

  public delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
