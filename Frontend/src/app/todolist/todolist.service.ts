import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo } from './todo.model';
import { environment } from '../../environments/environment';

@Injectable()
export class TodoListService {
  private base = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.base);
  }

  create(title: string): Observable<Todo> {
    return this.http.post<Todo>(this.base, { title });
  }

  update(id: string, todo: Todo): Observable<any> {
    return this.http.put(`${this.base}/${id}`, todo);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.base}/${id}`);
  }
}
